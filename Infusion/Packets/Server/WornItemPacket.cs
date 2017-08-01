using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class WornItemPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public ObjectId ItemId { get; private set; }
        public ModelId Type { get; private set; }
        public Layer Layer { get; private set; }
        public ObjectId PlayerId { get; private set; }
        public Color Color { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(1);
            ItemId = reader.ReadObjectId();
            Type = reader.ReadModelId();
            reader.Skip(1);
            Layer = reader.ReadLayer();
            PlayerId = reader.ReadObjectId();
            Color = reader.ReadColor();
        }
    }
}