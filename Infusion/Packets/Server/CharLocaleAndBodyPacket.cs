using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class CharLocaleAndBodyPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId PlayerId { get; private set; }

        public ModelId BodyType { get; set; }

        public Location3D Location { get; private set; }

        public Direction Direction { get; set; }

        public MovementType MovementType { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadObjectId();
            uint unknown1 = reader.ReadUInt();
            BodyType = reader.ReadModelId();
            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            byte unknown2 = reader.ReadByte();
            sbyte zloc = reader.ReadSByte();
            Location = new Location3D(xloc, yloc, zloc);
            (Direction, MovementType) = reader.ReadDirection();
        }

        public override Packet RawPacket => rawPacket;
    }
}
