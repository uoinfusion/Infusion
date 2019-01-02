using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class WearItemRequest
    {
        public WearItemRequest(ObjectId itemId, Layer layer, ObjectId playerId)
        {
            ItemId = itemId;
            Layer = layer;
            PlayerId = playerId;

            var payload = new byte[10];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.DropWearItem.Id);
            writer.WriteId(itemId);
            writer.WriteByte((byte) layer);
            writer.WriteId(playerId);

            RawPacket = new Packet(PacketDefinitions.DropWearItem.Id, payload);
        }

        public ObjectId ItemId { get; }
        public Layer Layer { get; }
        public ObjectId PlayerId { get; }

        public Packet RawPacket { get; }
    }
}