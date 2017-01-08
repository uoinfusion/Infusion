using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Client
{
    public class DropItemRequest
    {
        public uint ItemId { get; }

        public Location3D Location { get; }

        public uint ContainerId { get; }

        public DropItemRequest(uint itemId, uint containerId)
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
            writer.WriteUInt(ItemId);
            writer.WriteUShort(Location.X);
            writer.WriteUShort(Location.Y);
            writer.WriteByte(Location.Z);
            writer.WriteUInt(ContainerId);

            return new Packet(PacketDefinitions.DropItem.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
