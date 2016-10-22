using System;
using System.IO;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions.Client;

namespace UltimaRX
{
    public class UltimaClientConnection
    {
        private readonly IDiagnosticStream diagnosticStream;

        public UltimaClientConnection()
            : this(UltimaClientConnectionStatus.Initial, NullDiagnosticStream.Instance)
        {
        }

        public UltimaClientConnection(UltimaClientConnectionStatus status)
            : this(status, NullDiagnosticStream.Instance)
        {
        }

        public UltimaClientConnection(UltimaClientConnectionStatus status, IDiagnosticStream diagnosticStream)
        {
            this.diagnosticStream = diagnosticStream;
            Status = status;
        }

        public UltimaClientConnectionStatus Status { get; private set; }

        public event EventHandler<Packet> PacketReceived;

        public void ReceiveBatch(IPullStream inputStream)
        {
            diagnosticStream.BaseStream = inputStream;

            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                    ReceiveSeed(diagnosticStream);
                    Status = UltimaClientConnectionStatus.ServerLogin;
                    break;
                case UltimaClientConnectionStatus.PreGameLogin:
                    ReceiveSeed(diagnosticStream);
                    Status = UltimaClientConnectionStatus.GameLogin;
                    break;
            }

            foreach (var packet in PacketParser.ParseBatch(diagnosticStream))
            {
                OnPacketReceived(packet);
                switch (Status)
                {
                    case UltimaClientConnectionStatus.ServerLogin:
                        if (packet.Id == SelectServerRequestDefinition.Id)
                            Status = UltimaClientConnectionStatus.PreGameLogin;
                        break;
                    case UltimaClientConnectionStatus.GameLogin:
                        if (packet.Id == GameServerLoginRequestDefinition.Id)
                            Status = UltimaClientConnectionStatus.Game;
                        break;
                }
            }
        }

        private void ReceiveSeed(IPullStream inputStream)
        {
            var payload = new byte[4];
            inputStream.Read(payload, 0, 4);
            var packet = new Packet(-1, payload);
            OnPacketReceived(packet);
        }

        private void OnPacketReceived(Packet packet)
        {
            PacketReceived?.Invoke(this, packet);
            diagnosticStream.FinishPacket(packet);
        }

        public void Send(Packet packet, Stream outputStream)
        {
            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                case UltimaClientConnectionStatus.ServerLogin:
                    outputStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case UltimaClientConnectionStatus.Game:
                    var huffmanStream = new HuffmanStream(outputStream);
                    huffmanStream.Write(packet.Payload, 0, packet.Length);
                    break;
                default:
                    throw new NotImplementedException($"Sending packets while in {Status} Status.");
            }
        }
    }
}