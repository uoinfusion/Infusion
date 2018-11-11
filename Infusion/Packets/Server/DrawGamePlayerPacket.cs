using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class DrawGamePlayerPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public DrawGamePlayerPacket()
        {
        }

        public DrawGamePlayerPacket(ObjectId playerId, ModelId bodyType, Location3D location, Direction direction, MovementType movementType, Color color)
        {
            PlayerId = playerId;
            BodyType = bodyType;
            Location = location;
            Color = color;

            Serialize();
        }

        public void Serialize()
        {
            var payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.DrawGamePlayer.Id);
            writer.WriteId(PlayerId);
            writer.WriteModelId(BodyType);
            writer.WriteByte(0); // unknown
            writer.WriteColor(Color);
            writer.WriteByte(0); // flag, alway 0
            writer.WriteUShort((ushort)Location.X);
            writer.WriteUShort((ushort)Location.Y);
            writer.WriteUShort(0); // unknown
            writer.WriteMovement(Direction, MovementType);
            writer.WriteSByte((sbyte)Location.Z);

            rawPacket = new Packet(PacketDefinitions.DrawGamePlayer.Id, payload);
        }

        public byte Flags { get; set; }

        public ObjectId PlayerId { get; set; }

        public ModelId BodyType { get; set; }

        public Location3D Location { get; set; }

        public override Packet RawPacket => rawPacket;

        public Direction Direction { get; set; }

        public MovementType MovementType { get; set; }

        public Color Color { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadObjectId();
            BodyType = reader.ReadModelId();
            reader.Skip(1); // unknown

            Color = reader.ReadColor();
            Flags = reader.ReadByte();

            var xloc = reader.ReadUShort();
            var yloc = reader.ReadUShort();
            reader.Skip(2); // unknown
            (Direction, MovementType) = reader.ReadDirection();
            var zloc = reader.ReadSByte();

            Location = new Location3D(xloc, yloc, zloc);
        }
    }
}