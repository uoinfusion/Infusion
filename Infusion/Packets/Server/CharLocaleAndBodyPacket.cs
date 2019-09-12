using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class CharLocaleAndBodyPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId PlayerId { get; set; }

        public ModelId BodyType { get; set; }

        public Location3D Location { get; set; }

        public Direction Direction { get; set; }

        public MovementType MovementType { get; set; }

        public MapBoundary MapBoundary { get; set; }

        public Packet Serialize()
        {
            var payload = new byte[37];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.CharacterLocaleAndBody.Id);
            writer.WriteObjectId(PlayerId);
            writer.WriteUInt(0);
            writer.WriteModelId(BodyType);
            writer.WriteUShort((ushort)Location.X);
            writer.WriteUShort((ushort)Location.Y);
            writer.WriteByte(0);
            writer.WriteSByte((sbyte)Location.Z);
            writer.WriteDirection(Direction, MovementType);
            writer.WriteByte(0);
            writer.WriteInt(-1);

            //writer.WriteUShort(MapBoundary.MinX);
            //writer.WriteUShort(MapBoundary.MinY);
            //writer.WriteUShort(MapBoundary.MaxX);
            //writer.WriteUShort(MapBoundary.MaxY);

            rawPacket = new Packet(PacketDefinitions.CharacterLocaleAndBody.Id, payload);

            return rawPacket;
        }

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

            //reader.Skip(5);

            //var minX = reader.ReadUShort();
            //var minY = reader.ReadUShort();
            //var maxX = reader.ReadUShort();
            //var maxY = reader.ReadUShort();
            //MapBoundary = new MapBoundary(minX, minY, maxX, maxY);
        }

        public override Packet RawPacket => rawPacket;
    }
}
