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

        private Packet rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadObjectId();
            Type = reader.ReadModelId();
            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte());
            (Direction, MovementType) = reader.ReadDirection();
            Color = (Color) reader.ReadUShort();
        }

        public override Packet RawPacket => rawPacket;
    }
}
