using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class DeleteObjectPacket : MaterializedPacket
    {
        public DeleteObjectPacket()
        {
        }

        public DeleteObjectPacket(ObjectId id)
        {
            Id = id;

            var payload = new byte[5];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.DeleteObject.Id);
            writer.WriteId(id);

            rawPacket = new Packet(PacketDefinitions.DeleteObject.Id, payload);
        }

        public ObjectId Id { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            Id = ArrayPacketReader.ReadId(rawPacket.Payload, 1);
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
