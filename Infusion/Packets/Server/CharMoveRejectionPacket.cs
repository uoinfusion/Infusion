using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class CharMoveRejectionPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public byte SequenceKey { get; private set; }

        public Location3D Location { get; private set; }

        public Direction Direction { get; private set; }

        public MovementType MovementType { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            SequenceKey = reader.ReadByte();
            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            (Direction, MovementType) = reader.ReadDirection();
            byte zloc = reader.ReadByte();

            Location = new Location3D(xloc, yloc, zloc);
        }

        public override Packet RawPacket => this.rawPacket;
    }
}
