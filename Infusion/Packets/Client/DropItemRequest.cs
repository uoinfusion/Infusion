using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class DropItemRequest
    {
        public ObjectId ItemId { get; }

        public Location3D Location { get; }

        public ObjectId ContainerId { get; }

        public DropItemRequest(ObjectId itemId, ObjectId containerId)
            : this(itemId, containerId, new Location3D(0xFFFF, 0xFFFF, 0x00))
        {
        }

        public DropItemRequest(ObjectId itemId, ObjectId containerId, Location2D containerLocation)
            : this(itemId, containerId, new Location3D(containerLocation.X, containerLocation.Y, 0))
        {
        }

        public DropItemRequest(ObjectId itemId, Location3D location)
            : this(itemId, 0xFFFFFF, location)
        {
        }

        private DropItemRequest(ObjectId itemId, ObjectId containerId, Location3D location)
        {
            ItemId = itemId;
            ContainerId = containerId;
            Location = location;

            RawPacket = Serialize();
        }

        private Packet Serialize()
        {
            byte[] payload = new byte[14];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.DropItem.Id);
            writer.WriteId(ItemId);
            writer.WriteUShort((ushort)Location.X);
            writer.WriteUShort((ushort)Location.Y);
            writer.WriteSByte((sbyte)Location.Z);
            writer.WriteId(ContainerId);

            return new Packet(PacketDefinitions.DropItem.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
