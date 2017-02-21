using UltimaRX.IO;

namespace UltimaRX.Packets.Client
{
    public class WearItemRequest
    {
        public uint ItemId { get; }
        public Layer Layer { get; }
        public uint PlayerId { get; }

        public WearItemRequest(uint itemId, Layer layer, uint playerId)
        {
            ItemId = itemId;
            Layer = layer;
            PlayerId = playerId;

            var payload = new byte[10];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.DropWearItem.Id);
            writer.WriteUInt(itemId);
            writer.WriteByte((byte)layer);
            writer.WriteUInt(playerId);

            RawPacket = new Packet(PacketDefinitions.AttackRequest.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
