using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class UpdatePlayerPacket : MaterializedPacket
    {
        public ObjectId PlayerId { get; private set; }
        public ModelId Type { get; private set; }
        public Location3D Location { get; private set; }
        public Direction Direction { get; private set; }
        public MovementType MovementType { get; set; }
        public Color Color { get; private set; }
        public byte Flags { get; private set; }

        private Packet rawPacket;

        public UpdatePlayerPacket()
        {
        }

        public UpdatePlayerPacket(ObjectId id, ModelId type, Location3D location,
            Direction direction, Color color)
        {
            PlayerId = id;
            Type = type;
            Location = location;
            Direction = direction;
            MovementType = MovementType.Walk;
            Color = color;
            Flags = 0;

            byte[] payload = new byte[17];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.UpdatePlayer.Id);
            writer.WriteId(id);
            writer.WriteModelId(type);
            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);
            writer.WriteSByte((sbyte)location.Z);
            writer.WriteMovement(direction, MovementType.Walk);
            writer.WriteColor(color);
            writer.WriteByte(0);
            writer.WriteByte(0);

            rawPacket = new Packet(PacketDefinitions.UpdateCurrentStamina.Id, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadObjectId();
            Type = reader.ReadModelId();
            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            (Direction, MovementType) = reader.ReadDirection();
            Color = reader.ReadColor();
            Flags = reader.ReadByte();
        }

        public override Packet RawPacket => rawPacket;
    }
}
