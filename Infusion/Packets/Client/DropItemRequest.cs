using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class DropItemRequest
    {
        public ObjectId ItemId { get; }

        public Location3D Location { get; }

        public ObjectId ContainerId { get; }

        public DropItemRequest(ObjectId itemId, ObjectId containerId)
        {
            ItemId = itemId;
            ContainerId = containerId;
            Location = new Location3D(0xFFFF, 0xFFFF, 0x00);

            RawPacket = Serialize();
        }

        private Packet Serialize()
        {
            byte[] payload = new byte[14];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.DropItem.Id);
            writer.WriteId(ItemId);
            writer.WriteUShort(Location.X);
            writer.WriteUShort(Location.Y);
            writer.WriteByte(Location.Z);
            writer.WriteId(ContainerId);

            return new Packet(PacketDefinitions.DropItem.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
