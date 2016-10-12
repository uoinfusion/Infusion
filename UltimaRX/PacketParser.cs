using System;
using System.Collections.Generic;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;

namespace UltimaRX
{
    public static class PacketParser
    {
        public static IEnumerable<Packet> ParseBatch(byte[] batch, int position = 0)
        {
            while (position < batch.Length)
            {
                int packetId = batch[position];
                var packetLength = GetPacketLength(batch, position);

                var payload = new byte[packetLength];
                Array.Copy(batch, position, payload, 0, packetLength);
                position += packetLength;

                yield return new Packet(packetId, payload);
            }
        }

        private static int GetPacketLength(byte[] batch, int position)
        {
            int packedId = batch[position];
            PacketDefinition packetDefinition;

            if (PacketDefinitionRegistry.TryFind(packedId, out packetDefinition))
                return packetDefinition.GetSize(new ArrayPacketReader(batch, position));

            throw new NotImplementedException($"Unknown packet type {batch[position]:X2}");
        }
    }
}