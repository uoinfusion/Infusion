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

            while (position < inputDataEnumerator.Current.Length)
            {
                int packetId;
                int packetLength;

                switch (status)
                {
                    case UltimaClientConnectionStatus.BeforeInitialSeed:
                        packetId = -1;
                        packetLength = 4;
                        status = UltimaClientConnectionStatus.AfterInitialSeed;
                        break;
                    default:
                        packetId = inputDataEnumerator.Current[position];
                        packetLength = GetPacketLength(inputDataEnumerator.Current, position);
                        break;
                }

                var payload = new byte[packetLength];
                Array.Copy(inputDataEnumerator.Current, position, payload, 0, packetLength);
                PacketReceived?.Invoke(this, new Packet(packetId, payload));
                position += packetLength;
            }
        }

        private int GetPacketLength(byte[] current, int position)
        {
            int packedId = current[position];
            PacketDefinition packetDefinition;

            if (PacketDefinitionRegistry.TryFind(packedId, out packetDefinition))
                return packetDefinition.GetSize(new ArrayPacketReader(current, position));

            throw new NotImplementedException($"Unknown packet type{current[position]}");
        }
    }
}