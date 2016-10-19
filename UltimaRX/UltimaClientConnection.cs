using System;
using System.Collections.Generic;
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
                status = UltimaClientConnectionStatus.PreLogin;
                position += 4;
            }

            foreach (var packet in PacketParser.ParseBatch(inputDataEnumerator.Current, position))
                PacketReceived?.Invoke(this, packet);
        }

        public byte[] Transform(Packet packet)
        {
            return packet.Payload;
        }
    }
}