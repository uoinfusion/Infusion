using System;
using Infusion.IO;

namespace Infusion.Packets
{
    internal class PacketDefinition<TPacket> : PacketDefinition
        where TPacket : MaterializedPacket
    {
        private readonly Func<TPacket> factory;

        public PacketDefinition(int id, PacketLength length, string name, Func<TPacket> factory)
            : base(id, length, name)
        {
            this.factory= factory;
        }

        protected override MaterializedPacket MaterializeImpl(Packet rawPacket) => factory();
        public override MaterializedPacket Instantiate() => factory();
    }

    internal class PacketDefinition
    {
        public PacketDefinition(int id, PacketLength length, string name)
        {
            Id = id;
            Length = length;
            Name = name;
        }

        public int Id { get; }

        private PacketLength Length { get; }
        public string Name { get; }

        public int GetSize(IPacketReader reader)
        {
            return Length.GetSize(reader);
        }

        public virtual MaterializedPacket Instantiate() => throw new NotImplementedException();

        public MaterializedPacket Materialize(Packet rawPacket)
        {
            if (rawPacket.Id != Id)
            {
                throw new InvalidOperationException(
                    message: $"Cannot materialize rawPacket because it's id is {rawPacket.Id} but {Id} is expected");
            }

            var materializedPacket = MaterializeImpl(rawPacket);

            try
            {
                materializedPacket.Deserialize(rawPacket);

            }
            catch (Exception e)
            {
                throw new PacketMaterializationException($"Packet id = {rawPacket.Id}", e);
            }
            return materializedPacket;
        }

        protected virtual MaterializedPacket MaterializeImpl(Packet rawPacket)
        {
            throw new NotImplementedException();
        }
    }
}