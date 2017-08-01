using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class SingleClickRequest
    {
        public SingleClickRequest(ObjectId itemId)
        {
            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.SingleClick.Id);
            writer.WriteId(itemId);
            RawPacket = new Packet(PacketDefinitions.SingleClick.Id, payload);

            ItemId = itemId;
        }

        public Packet RawPacket { get; }
        public ObjectId ItemId { get; }
    }
}