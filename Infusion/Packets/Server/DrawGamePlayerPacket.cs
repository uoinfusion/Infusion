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
            var payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            PlayerId = playerId;
            BodyType = bodyType;
            Location = location;
            Color = color;

            writer.WriteByte((byte) PacketDefinitions.DrawGamePlayer.Id);
            writer.WriteId(playerId);
            writer.WriteModelId(bodyType);
            writer.WriteByte(0); // unknown
            writer.WriteColor(color);
            writer.WriteByte(0); // flag, alway 0
            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);
            writer.WriteUShort(0); // unknown
            writer.WriteMovement(direction, movementType);
            writer.WriteSByte((sbyte)location.Z);

            rawPacket = new Packet(PacketDefinitions.DrawGamePlayer.Id, payload);
        }

        public byte Flags { get; private set; }

        public ObjectId PlayerId { get; private set; }

        public ModelId BodyType { get; private set; }

        public Location3D Location { get; private set; }

        public override Packet RawPacket => rawPacket;

        public Direction Direction { get; set; }

        public MovementType MovementType { get; set; }

        public Color Color { get; private set; }

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
            var zloc = reader.ReadByte();

            Location = new Location3D(xloc, yloc, zloc);
        }
    }
}