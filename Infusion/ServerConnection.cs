using System;
using System.IO;
using System.Linq;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion
{
    internal sealed class ServerConnection
    {
        private readonly IDiagnosticPullStream diagnosticPullStream;
        private readonly IDiagnosticPushStream diagnosticPushStream;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly HuffmanStream huffmanStream;
        private readonly NewGameStream receiveNewGameStream;
        private readonly NewGameStream sendNewGameStream;
        private readonly PullStreamToStreamAdapter preLoginStream;
        private readonly LoginStream loginStream;

        public ServerConnectionStatus Status { get; private set; }

        public ServerConnection()
            : this(ServerConnectionStatus.PreLogin, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance, true,
                  PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        public ServerConnection(ServerConnectionStatus status)
            : this(status, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance, true,
                  PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream)
            : this (status, diagnosticPullStream, diagnosticPushStream, true, PacketDefinitionRegistryFactory.CreateClassicClient())
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream, bool encrypted, PacketDefinitionRegistry packetRegistry)
        {
            this.Status = status;
            this.diagnosticPullStream = diagnosticPullStream;
            this.diagnosticPushStream = diagnosticPushStream;
            this.packetRegistry = packetRegistry;
            this.loginStream = new LoginStream(null, encrypted);

            this.receiveNewGameStream = new NewGameStream(new byte[] { 127, 0, 0, 1 }, encrypted);
            this.sendNewGameStream = new NewGameStream(new byte[] { 127, 0, 0, 1 }, encrypted);

            huffmanStream = new HuffmanStream(receiveNewGameStream);
            preLoginStream = new PullStreamToStreamAdapter(diagnosticPullStream);
        }

        public event EventHandler<Packet> PacketReceived;
        private readonly byte[] receiveBuffer = new byte[65535];

        public void Receive(IPullStream inputStream)
        {
            var processingStream = GetReceiveStream(inputStream);

            while (inputStream.DataAvailable)
            {
                var packetReader = new StreamPacketReader(processingStream, receiveBuffer);
                int packetId = packetReader.ReadByte();
                if ((packetId < 0) || (packetId > 255))
                    throw new EndOfStreamException();

                diagnosticPullStream.StartPacket();
                var packetDefinition = packetRegistry.Find(packetId);
                var packetSize = packetDefinition.GetSize(packetReader);
                packetReader.ReadBytes(packetSize - packetReader.Position);
                var payload = new byte[packetSize];
                Array.Copy(receiveBuffer, 0, payload, 0, packetSize);

                var packet = new Packet(packetId, payload);

                if (packetId == PacketDefinitions.ConnectToGameServer.Id)
                {
                    Status = ServerConnectionStatus.PreGame;
                }

                diagnosticPullStream.FinishPacket(packet);

                OnPacketReceived(packet);
            }
        }

        private void OnPacketReceived(Packet packet)
        {
            PacketReceived?.Invoke(this, packet);
        }

        private Stream GetReceiveStream(IPullStream inputStream)
        {
            diagnosticPullStream.BaseStream = inputStream;

            switch (Status)
            {
                case ServerConnectionStatus.PreLogin:
                    return preLoginStream;

                case ServerConnectionStatus.Game:
                    receiveNewGameStream.BasePullStream = diagnosticPullStream;

                    return huffmanStream;

                default:
                    throw new NotImplementedException($"Unknown Status: {Status}");
            }
        }

        public void Send(Packet packet, Stream outputStream)
        {
            diagnosticPushStream.DumpPacket(packet);

            switch (Status)
            {
                case ServerConnectionStatus.Initial:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);

                    var seed = packet.Payload.Length > 4
                        ? BitConverter.ToUInt32(packet.Payload.Skip(1).Take(4).Reverse().ToArray(), 0)
                        : BitConverter.ToUInt32(packet.Payload.Take(4).Reverse().ToArray(), 0);
                    loginStream.SetSeed(seed);

                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    Status = ServerConnectionStatus.PreLogin;
                    break;
                case ServerConnectionStatus.PreLogin:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    loginStream.BaseStream = new PushStreamToStreamAdapter(diagnosticPushStream);
                    loginStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case ServerConnectionStatus.PreGame:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    Status = ServerConnectionStatus.Game;
                    break;
                case ServerConnectionStatus.Game:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    sendNewGameStream.BasePushStream = new PushStreamToStreamAdapter(diagnosticPushStream);
                    sendNewGameStream.Write(packet.Payload, 0, packet.Length);
                    break;
            }

            diagnosticPushStream.Finish();
        }
    }
}