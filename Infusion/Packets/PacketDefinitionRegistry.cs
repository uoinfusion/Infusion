using System;
using System.Collections.Generic;

namespace Infusion.Packets
{
    internal class PacketDefinitionRegistry
    {
        private readonly Dictionary<int, PacketDefinition> definitionsById = new Dictionary<int, PacketDefinition>();
        private readonly Dictionary<Type, PacketDefinition> definitionsByType = new Dictionary<Type, PacketDefinition>();

        public void Register(PacketDefinition definition)
        {
            definitionsById[definition.Id] = definition;
        }

        public void Register<T>(PacketDefinition<T> definition) where T : MaterializedPacket
        {
            definitionsById[definition.Id] = definition;
            definitionsByType[typeof(T)] = definition;
        }

        public PacketDefinition Find(int id)
        {
            PacketDefinition definition;

            if (!definitionsById.TryGetValue(id, out definition))
            {
                throw new InvalidOperationException($"Unknown packet id {id:X2}.");
            }

            return definition;
        }

        public PacketDefinition Find<T>()
        {
            PacketDefinition definition;

            if (!definitionsByType.TryGetValue(typeof(T), out definition))
            {
                throw new InvalidOperationException($"Unknown packet {typeof(T).FullName}.");
            }

            return definition;
        }

        internal void Reset()
        {
            definitionsById.Clear();
            definitionsByType.Clear();
        }

        public bool TryFind(int packedId, out PacketDefinition packetDefinition)
        {
            return definitionsById.TryGetValue(packedId, out packetDefinition);
        }

        public T Materialize<T>(Packet rawPacket) where T : MaterializedPacket
        {
            var definition = Find(rawPacket.Id);
            return (T)definition.Materialize(rawPacket);
        }

        public T Instantiate<T>() where T : MaterializedPacket
        {
            var definition = Find<T>();
            return (T)definition.Instantiate();
        }
    }
}