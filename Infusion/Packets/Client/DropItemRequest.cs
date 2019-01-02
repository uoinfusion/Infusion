using System;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class DropItemRequest : MaterializedPacket
    {
        public ObjectId ItemId { get; private set;  }

        public Location3D Location { get; private set; }

        public ObjectId ContainerId { get; private set; }

        private Packet rawPacket;
        private readonly bool useGridIndex;
        private readonly int packetLength;

        public DropItemRequest(bool useGridIndex, int packetLength)
        {
            this.useGridIndex = useGridIndex;
            this.packetLength = packetLength;
        }

        public Packet Serialize(ObjectId itemId, ObjectId targetContainerId)
            => Serialize(itemId, targetContainerId, new Location3D(0xFFFF, 0xFFFF, 0x00));
        public Packet Serialize(ObjectId itemId, Location3D location)
            => Serialize(itemId, 0xFFFFFF, location);
        public Packet Serialize(ObjectId itemId, ObjectId targetContainerId, Location2D location)
            => Serialize(itemId, targetContainerId, (Location3D)location);


        public Packet Serialize(ObjectId itemId, ObjectId containerId, Location3D location)
        {
            ItemId = itemId;
            ContainerId = containerId;
            Location = location;

            byte[] payload = new byte[packetLength];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.DropItem.Id);
            writer.WriteId(ItemId);
            writer.WriteUShort((ushort)Location.X);
            writer.WriteUShort((ushort)Location.Y);
            writer.WriteSByte((sbyte)Location.Z);

            if (useGridIndex)
                writer.WriteByte(0);

            writer.WriteId(ContainerId);

            rawPacket = new Packet(PacketDefinitions.DropItem.Id, payload);
            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket) => throw new System.NotImplementedException();

        public override Packet RawPacket => rawPacket;
    }
}
