using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class DrawGamePlayerPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public DrawGamePlayerPacket()
        {
        }

        public DrawGamePlayerPacket(uint playerId, ModelId bodyType, Location3D location, Movement movement, Color color)
        {
            var payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            PlayerId = playerId;
            BodyType = bodyType;
            Location = location;
            Movement = movement;
            Color = color;

            writer.WriteByte((byte) PacketDefinitions.DrawGamePlayer.Id);
            writer.WriteUInt(playerId);
            writer.WriteModelId(bodyType);
            writer.WriteByte(0); // unknown
            writer.WriteColor(color);
            writer.WriteByte(0); // flag, alway 0
            writer.WriteUShort(location.X);
            writer.WriteUShort(location.Y);
            writer.WriteUShort(0); // unknown
            writer.WriteMovement(movement);
            writer.WriteByte(location.Z);

            rawPacket = new Packet(PacketDefinitions.DrawGamePlayer.Id, payload);
        }

        public uint PlayerId { get; private set; }

        public ModelId BodyType { get; private set; }

        public Location3D Location { get; private set; }

        public override Packet RawPacket => rawPacket;

        public Movement Movement { get; private set; }

        public Color Color { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            BodyType = reader.ReadModelId();
            reader.Skip(1); // unknown

            Color = reader.ReadColor();
            reader.Skip(1); // flag byte

            var xloc = reader.ReadUShort();
            var yloc = reader.ReadUShort();
            reader.Skip(2); // unknown
            Movement = (Movement) reader.ReadByte();
            var zloc = reader.ReadByte();

            Location = new Location3D(xloc, yloc, zloc);
        }
    }
}