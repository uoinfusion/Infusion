using System;
using Infusion.IO;

namespace Infusion.Packets
{
    // TODO: move materialization from packet to a materializer so it is possible to implement different protocol versions (two versions of the same packet)
    public class PacketDefinition<TPacket> : PacketDefinition
        where TPacket : MaterializedPacket
    {
        private readonly Func<Packet, TPacket> materializer;

        public PacketDefinition(int id, PacketLength length, string name, Func<Packet, TPacket> materializer)
            : base(id, length, name)
        {
            this.materializer = materializer;
        }

        protected override MaterializedPacket MaterializeImpl(Packet rawPacket)
        {
            return materializer(rawPacket);
        }
    }

    public class PacketDefinition
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