using System;
using System.IO;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public class ServerConnection
    {
        private readonly IDiagnosticPullStream diagnosticPullStream;
        private readonly IDiagnosticPushStream diagnosticPushStream;
        private readonly HuffmanStream huffmanStream;
        private readonly NewGameStream newGameStream = new NewGameStream(new byte[] {127, 0, 0, 1});
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
            huffmanStream = new HuffmanStream(newGameStream);
            preLoginStream = new PullStreamToStreamAdapter(diagnosticPullStream);
        }

        public event EventHandler<Packet> PacketReceived;

        public void Receive(IPullStream inputStream)
        {
            var processingStream = GetProcessingStream(inputStream);

            var received = new byte[1024];

            while (inputStream.DataAvailable)
            {
                var packetReader = new StreamPacketReader(processingStream, received);
                int packetId = packetReader.ReadByte();
                if ((packetId < 0) || (packetId > 255))
                    throw new EndOfStreamException();

                var packetDefinition = PacketDefinitionRegistry.Find(packetId);
                var packetSize = packetDefinition.GetSize(packetReader);
                packetReader.ReadBytes(packetSize - packetReader.Position);
                var payload = new byte[packetSize];
                Array.Copy(received, 0, payload, 0, packetSize);

                var packet = new Packet(packetId, payload);

                diagnosticPullStream.FinishPacket(packet);

                OnPacketReceived(packet);
            }
        }

        private void OnPacketReceived(Packet packet)
        {
            if ((Status == ServerConnectionStatus.PreLogin) && (packet.Id == 0x8C))
                Status = ServerConnectionStatus.Game;

            PacketReceived?.Invoke(this, packet);
        }

        private Stream GetProcessingStream(IPullStream inputStream)
        {
            diagnosticPullStream.BaseStream = inputStream;

            switch (Status)
            {
                case ServerConnectionStatus.PreLogin:
                    return preLoginStream;

                case ServerConnectionStatus.Game:
                    newGameStream.BasePullStream = diagnosticPullStream;

                    return huffmanStream;

                default:
                    throw new NotImplementedException($"Unknown Status: {Status}");
            }
        }

        public void Send(Packet packet, Stream outputStream)
        {
            switch (Status)
            {
                case ServerConnectionStatus.Initial:
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(outputStream);
                    // TODO: use seed sent by client
                    diagnosticPushStream.Write(new byte[] { 0xA9, 0xFE, 0x50, 0x50 }, 0, 4);
                    Status = ServerConnectionStatus.PreLogin;
                    break;
                case ServerConnectionStatus.PreLogin:
                    var loginStream = new LoginStream(outputStream);
                    loginStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case ServerConnectionStatus.Game:
                    var stream = new NewGameStream(new byte[] {127, 0, 0, 1}) {BasePushStream = outputStream};
                    diagnosticPushStream.BaseStream = new StreamToPushStreamAdapter(stream);
                    diagnosticPushStream.Write(packet.Payload, 0, packet.Length);
                    break;
            }

            diagnosticPushStream.FinishPacket(packet);
        }
    }
}