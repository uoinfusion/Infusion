using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class PickupItemRequest
    {
        public uint ItemId { get; }

        public ushort Count { get; }

        public PickupItemRequest(uint itemId, ushort count)
        {
            ItemId = itemId;
            Count = count;

            byte[] payload = new byte[7];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.PickUpItem.Id);
            writer.WriteUInt(itemId);
            writer.WriteUShort(count);

            RawPacket = new Packet(PacketDefinitions.PickUpItem.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
