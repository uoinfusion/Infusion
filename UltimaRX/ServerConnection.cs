using System;
using System.IO;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public class ServerConnection
    {
        private readonly IDiagnosticStream diagnosticStream;
        private readonly HuffmanStream huffmanStream;
        private readonly NewGameStream newGameStream = new NewGameStream(new byte[] {127, 0, 0, 1});
        private readonly PullStreamToStreamAdapter preLoginStream;
        private ServerConnectionStatus status;

        public ServerConnection() : this(ServerConnectionStatus.PreLogin, new NullDiagnosticStream())
        {
        }

        public ServerConnection(ServerConnectionStatus status, IDiagnosticStream diagnosticStream)
        {
            this.status = status;
            this.diagnosticStream = diagnosticStream;
            huffmanStream = new HuffmanStream(newGameStream);
            preLoginStream = new PullStreamToStreamAdapter(diagnosticStream);
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

                diagnosticStream.FinishPacket(packet);

                OnPacketReceived(packet);
            }
        }

        private void OnPacketReceived(Packet packet)
        {
            if ((status == ServerConnectionStatus.PreLogin) && (packet.Id == 0x8C))
                status = ServerConnectionStatus.Game;

            PacketReceived?.Invoke(this, packet);
        }

        private Stream GetProcessingStream(IPullStream inputStream)
        {
            diagnosticStream.BaseStream = inputStream;

            switch (status)
            {
                case ServerConnectionStatus.PreLogin:
                    return preLoginStream;

                case ServerConnectionStatus.Game:
                    newGameStream.BasePullStream = diagnosticStream;

                    return huffmanStream;

                default:
                    throw new NotImplementedException($"Unknown Status: {status}");
            }
        }

        public void Send(Packet packet, Stream outputStream)
        {
            switch (status)
            {
                case ServerConnectionStatus.PreLogin:
                    outputStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case ServerConnectionStatus.Game:
                    var stream = new NewGameStream(new byte[] {127, 0, 0, 1}) {BasePushStream = outputStream};
                    stream.Write(packet.Payload, 0, packet.Length);
                    break;
            }
        }
    }
}