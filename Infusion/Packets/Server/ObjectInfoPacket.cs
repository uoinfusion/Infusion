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

        public ObjectInfoPacket(ObjectId id, ModelId type, Location3D location, Color? color, ushort? amount = null)
        {
            Id = id;
            Type = type;
            Location = location;

            ushort packetLength = 15;
            if (color.HasValue)
                packetLength += 2;
            if (amount.HasValue)
            {
                packetLength += 2;
                id += 0x80000000;
            }

            var payload = new byte[packetLength];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte) PacketDefinitions.ObjectInfo.Id);
            writer.WriteUShort(packetLength);
            writer.WriteId(id);
            writer.WriteModelId(type);
            if (amount.HasValue)
                writer.WriteUShort(amount.Value);
            writer.WriteUShort((ushort)location.X);
            ushort y = (ushort)(color.HasValue ? location.Y | 0x8000 : location.Y);
            writer.WriteUShort(y);
            writer.WriteSByte((sbyte)location.Z);

            if (color.HasValue)
                writer.WriteColor(color.Value);

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

            sbyte zloc = reader.ReadSByte();

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
