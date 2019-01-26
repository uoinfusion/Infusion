using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class SingleClickRequest : MaterializedPacket
    {
        public Packet rawPacket;

        public override Packet RawPacket => rawPacket;
        public ObjectId ItemId { get; set; }

        public SingleClickRequest()
        {
        }

        public SingleClickRequest(ObjectId itemId)
        {
            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.SingleClick.Id);
            writer.WriteId(itemId);
            rawPacket = new Packet(PacketDefinitions.SingleClick.Id, payload);

            ItemId = itemId;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            ItemId = reader.ReadObjectId();
        }
    }
}