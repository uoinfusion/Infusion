using System;
using System.Collections.Generic;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public class UltimaClientConnection
    {
        private readonly IEnumerator<byte[]> inputDataEnumerator;
        private UltimaClientConnectionStatus status = UltimaClientConnectionStatus.BeforeInitialSeed;

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

            if (status == UltimaClientConnectionStatus.BeforeInitialSeed)
            {
                var payload = new byte[4];
                Array.Copy(inputDataEnumerator.Current, 0, payload, 0, 4);
                PacketReceived?.Invoke(this, new Packet(-1, payload));
                status = UltimaClientConnectionStatus.AfterInitialSeed;
                position += 4;
            }

            foreach (var packet in PacketParser.ParseBatch(inputDataEnumerator.Current, position))
                PacketReceived?.Invoke(this, packet);
        }
    }
}