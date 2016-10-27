using System;
using UltimaRX.IO;

namespace UltimaRX.Packets
{
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
                    $"Cannot materialize rawPacket because it's id is {rawPacket.Id} but {Id} is expected");
            }

            var materializedPacket = MaterializeImpl(rawPacket);

            materializedPacket.Deserialize(rawPacket);

            return materializedPacket;
        }

        protected virtual MaterializedPacket MaterializeImpl(Packet rawPacket)
        {
            throw new NotImplementedException();
        }
    }
}