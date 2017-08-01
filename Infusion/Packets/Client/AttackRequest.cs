using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class AttackRequest
    {
        private readonly ObjectId targetId;

        public AttackRequest(ObjectId targetId)
        {
            this.targetId = targetId;

            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.AttackRequest.Id);
            writer.WriteId(targetId);

            RawPacket = new Packet(PacketDefinitions.AttackRequest.Id, payload);
        }

        public ObjectId TargetId { get; }

        public Packet RawPacket { get; }
    }
}