using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class ObjectInfoPacket : MaterializedPacket
    {
        public ModelId Type { get; private set; }

        public ObjectId Id { get; private set; }

        public ushort Amount { get; private set; }

        public Location3D Location { get; private set; }

        public ObjectFlag Flags { get; private set; }

        public Direction Facing { get; private set; }

        public ObjectInfoPacket()
        {            
        }

        public ObjectInfoPacket(ObjectId id, ModelId type, Location3D location)
        {
            Id = id;
            Type = type;
            Location = location;

            var payload = new byte[15];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte) PacketDefinitions.ObjectInfo.Id);
            writer.WriteUShort(15);
            writer.WriteId(id);
            writer.WriteModelId(type);
            writer.WriteUShort(location.X);
            writer.WriteUShort(location.Y);
            writer.WriteByte(location.Z);

            rawPacket = new Packet(PacketDefinitions.ObjectInfo.Id, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            uint rawId = reader.ReadUInt();
            ushort rawType = reader.ReadUShort();

            uint finalId;
            if ((rawId & 0x80000000) != 0)
            {
                finalId = rawId - 0x80000000;
                Amount = reader.ReadUShort();
            }
            else
            {
                Amount = 1;
                finalId = rawId;
            }
            Id = new ObjectId(finalId);

            if ((rawType & 0x8000) != 0)
            {
                throw new PacketParsingException(rawPacket, "Not implementated: Type & 0x8000");
            }

            Type = rawType;

            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();

            if ((xloc & 0x8000) != 0)
            {
                xloc -= 0x8000;

                (Facing, _) = reader.ReadDirection();
            }

            byte zloc = reader.ReadByte();

            if ((yloc & 0x8000) != 0)
            {
                yloc -= 0x8000;
                Dye = (Color) reader.ReadUShort();
            }

            if ((yloc & 0x4000) != 0)
            {
                yloc -= 0x4000;
                Flags = (ObjectFlag)reader.ReadByte();
            }

            Location = new Location3D(xloc, yloc, zloc);
        }

        public Color Dye { get; private set; }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
