using System;
using System.Collections.Generic;
using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Parsers
{
    internal class PacketLogParser
    {
        private readonly PacketDefinitionRegistry packetRegistry;

        public PacketLogParser(PacketDefinitionRegistry packetRegistry)
        {
            this.packetRegistry = packetRegistry;
        }

        public IEnumerable<Packet> ParseBatch(IPullStream inputStream)
        {
            while (inputStream.DataAvailable)
            {
                yield return ParsePacket(inputStream);
            }
        }

        public Packet ParsePacket(IPullStream inputStream)
        {
            byte[] buffer = new byte[65535];

            var reader = new StreamPacketReader(new PullStreamToStreamAdapter(inputStream), buffer);

            int packetId = reader.ReadByte();
            int packetLength = GetPacketLength(reader, packetId);
            reader.ReadBytes(packetLength - reader.Position);

            var payload = new byte[packetLength];
            Array.Copy(buffer, 0, payload, 0, packetLength);

            return new Packet(packetId, payload);
        }

        private int GetPacketLength(StreamPacketReader reader, int packetId)
        {
            PacketDefinition packetDefinition;

            if (packetRegistry.TryFind(packetId, out packetDefinition))
                return packetDefinition.GetSize(reader);

            throw new NotImplementedException($"Unknown packet type {packetId:X2}");
        }
    }
}