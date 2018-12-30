using System;
using System.Collections.Generic;

namespace Infusion.Packets
{
    internal class PacketDefinitionRegistry
    {
        private readonly Dictionary<int, PacketDefinition> Definitions = new Dictionary<int, PacketDefinition>();

        public void Register(PacketDefinition definition)
        {
            Definitions[definition.Id] = definition;
        }

        public PacketDefinition Find(int id)
        {
            PacketDefinition definition;

            if (!Definitions.TryGetValue(id, out definition))
            {
                throw new InvalidOperationException($"Unknown packet id {id:X2}.");
            }

            return definition;
        }

        public bool TryFind(int packedId, out PacketDefinition packetDefinition)
        {
            return Definitions.TryGetValue(packedId, out packetDefinition);
        }

        public T Materialize<T>(Packet rawPacket) where T : MaterializedPacket
        {
            PacketDefinition definition = Find(rawPacket.Id);
            return (T)definition.Materialize(rawPacket);
        }
    }
}