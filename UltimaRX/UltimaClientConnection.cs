using System;
using System.Collections.Generic;
using System.IO;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions.Client;

namespace UltimaRX
{
    public class UltimaClientConnection
    {
        private readonly IEnumerator<byte[]> inputDataEnumerator;

        public UltimaClientConnection(IEnumerable<byte[]> inputData)
        {
            inputDataEnumerator = inputData.GetEnumerator();
        }

        public UltimaClientConnection(IEnumerable<byte[]> inputData, UltimaClientConnectionStatus status)
            : this(inputData)
        {
            Status = status;
        }

        public UltimaClientConnectionStatus Status { get; private set; } = UltimaClientConnectionStatus.Initial;

        public event EventHandler<Packet> PacketReceived;

        public void ReceiveBatch()
        {
            if (!inputDataEnumerator.MoveNext())
                return;

            var position = 0;

            switch (Status)
            {
                case UltimaClientConnectionStatus.Initial:
                    position = ReceiveSeed(position);
                    Status = UltimaClientConnectionStatus.ServerLogin;
                    break;
                case UltimaClientConnectionStatus.PreGameLogin:
                    position = ReceiveSeed(position);
                    Status = UltimaClientConnectionStatus.GameLogin;
                    break;
            }

            foreach (var packet in PacketParser.ParseBatch(inputDataEnumerator.Current, position))
            {
                PacketReceived?.Invoke(this, packet);

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

        private int ReceiveSeed(int position)
        {
            var payload = new byte[4];
            Array.Copy(inputDataEnumerator.Current, 0, payload, 0, 4);
            PacketReceived?.Invoke(this, new Packet(-1, payload));
            position += 4;
            return position;
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