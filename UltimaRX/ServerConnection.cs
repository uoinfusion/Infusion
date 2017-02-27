using System;
using System.IO;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion
{
    public class ServerConnection
    {
        private readonly IDiagnosticPullStream diagnosticPullStream;
        private readonly IDiagnosticPushStream diagnosticPushStream;
        private readonly HuffmanStream huffmanStream;
        private readonly NewGameStream receiveNewGameStream = new NewGameStream(new byte[] { 127, 0, 0, 1 });
        private readonly NewGameStream sendNewGameStream = new NewGameStream(new byte[] { 127, 0, 0, 1 });
        private readonly PullStreamToStreamAdapter preLoginStream;

        public ServerConnectionStatus Status { get; private set; }

        public ServerConnection()
            : this(ServerConnectionStatus.PreLogin, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance
            )
        {
        }

        public ServerConnection(ServerConnectionStatus status)
            : this(status, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance)
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticPullStream diagnosticPullStream,
            IDiagnosticPushStream diagnosticPushStream)
        {
            this.Status = status;
            this.diagnosticPullStream = diagnosticPullStream;
            this.diagnosticPushStream = diagnosticPushStream;
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

                var packetDefinition = PacketDefinitionRegistry.Find(packetId);
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

        private readonly LoginStream loginStream = new LoginStream(null);

        public void Send(Packet packet, Stream outputStream)
        {
            diagnosticPushStream.DumpPacket(packet);

            switch (Status)
            {
                case ServerConnectionStatus.Initial:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    // TODO: use seed sent by client
                    diagnosticPushStream.Write(new byte[] { 0xA9, 0xFE, 0x50, 0x50 }, 0, 4);
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