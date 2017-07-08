using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class SingleClickRequest
    {
        public SingleClickRequest(uint itemId)
        {
            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.SingleClick.Id);
            writer.WriteUInt(itemId);
            RawPacket = new Packet(PacketDefinitions.SingleClick.Id, payload);

            ItemId = itemId;
        }

        public Packet RawPacket { get; }
        public uint ItemId { get; }
    }
}