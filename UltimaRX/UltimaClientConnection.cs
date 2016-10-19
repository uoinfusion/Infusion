using System;
using System.Collections.Generic;
using System.IO;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public class UltimaClientConnection
    {
        private readonly IEnumerator<byte[]> inputDataEnumerator;
        private UltimaClientConnectionStatus status = UltimaClientConnectionStatus.Initial;

        public UltimaClientConnection(IEnumerable<byte[]> inputData)
        {
            inputDataEnumerator = inputData.GetEnumerator();
        }

        public UltimaClientConnection(IEnumerable<byte[]> inputData, UltimaClientConnectionStatus status)
            : this(inputData)
        {
            this.status = status;
        }

        public event EventHandler<Packet> PacketReceived;

        public void ReceiveBatch()
        {
            if (!inputDataEnumerator.MoveNext())
                return;

            var position = 0;

            if (status == UltimaClientConnectionStatus.Initial)
            {
                var payload = new byte[4];
                Array.Copy(inputDataEnumerator.Current, 0, payload, 0, 4);
                PacketReceived?.Invoke(this, new Packet(-1, payload));
                position += 4;
            }

            foreach (var packet in PacketParser.ParseBatch(inputDataEnumerator.Current, position))
                PacketReceived?.Invoke(this, packet);
        }

        public void Send(Packet packet, Stream outputStream)
        {
            switch (status)
            {
                case UltimaClientConnectionStatus.PreLogin:
                    outputStream.Write(packet.Payload, 0, packet.Length);
                    break;
                case UltimaClientConnectionStatus.Game:
                    var huffmanStream = new HuffmanStream(outputStream);
                    huffmanStream.Write(packet.Payload, 0, packet.Length);
                    break;
                default:
                    throw new NotImplementedException($"Sending packets while in {status} status.");
            }
        }
    }
}