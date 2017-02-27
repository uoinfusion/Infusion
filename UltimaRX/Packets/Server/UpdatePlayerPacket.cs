using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class UpdatePlayerPacket : MaterializedPacket
    {
        public uint PlayerId { get; private set; }
        public ModelId Type { get; private set; }
        public Location3D Location { get; private set; }
        public Movement Direction { get; private set; }
        public Color Color { get; private set; }

        private Packet rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            Type = reader.ReadModelId();
            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadByte());
            Direction = (Movement) reader.ReadByte();
            Color = (Color) reader.ReadUShort();
        }

        public override Packet RawPacket => rawPacket;
    }
}
