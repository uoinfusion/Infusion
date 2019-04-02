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

        public CharMoveRejectionPacket()
        {
        }

        public CharMoveRejectionPacket(byte sequenceKey, Location3D location, Direction direction)
        {
            SequenceKey = sequenceKey;
            Location = location;
            Direction = direction;

            var payload = new byte[8];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.CharMoveRejection.Id);
            writer.WriteByte(sequenceKey);
            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);
            writer.WriteByte((byte)direction);
            writer.WriteByte((byte)location.Z);

            rawPacket = new Packet(PacketDefinitions.CharMoveRejection.Id, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            SequenceKey = reader.ReadByte();
            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            (Direction, MovementType) = reader.ReadDirection();
            sbyte zloc = reader.ReadSByte();

            Location = new Location3D(xloc, yloc, zloc);
        }

        public override Packet RawPacket => this.rawPacket;
    }
}
