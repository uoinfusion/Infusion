using System;
using UltimaRX.IO;

namespace UltimaRX.Packets.PacketDefinitions
{
    public abstract class PacketDefinition
    {
        protected PacketDefinition(int id, PacketLength length)
        {
            Id = id;
            Length = length;
        }

        public int Id { get; }

        private PacketLength Length { get; }

        public int GetSize(IPacketReader reader)
        {
            return Length.GetSize(reader);
        }

        public MaterializedPacket Materialize(Packet rawPacket)
        {
            if (rawPacket.Id != Id)
            {
                throw new InvalidOperationException($"Cannot materialize rawPacket because it's id is {rawPacket.Id} but {Id} is expected");
            }

            return MaterializeImpl(rawPacket);
        }

        protected virtual MaterializedPacket MaterializeImpl(Packet rawPacket)
        {
            throw new NotImplementedException();
        }
    }
}
