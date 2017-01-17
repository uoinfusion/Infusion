using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class WornItemPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public uint ItemId { get; private set; }
        public ModelId Type { get; private set; }
        public Layer Layer { get; private set; }
        public uint PlayerId { get; private set; }
        public Color Color { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(1);
            ItemId = reader.ReadUInt();
            Type = reader.ReadModelId();
            reader.Skip(1);
            Layer = reader.ReadLayer();
            PlayerId = reader.ReadUInt();
            Color = reader.ReadColor();
        }
    }
}