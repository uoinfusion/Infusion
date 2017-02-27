using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class AttackRequest
    {
        private readonly uint targetId;

        public AttackRequest(uint targetId)
        {
            this.targetId = targetId;

            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.AttackRequest.Id);
            writer.WriteUInt(targetId);

            RawPacket = new Packet(PacketDefinitions.AttackRequest.Id, payload);
        }

        public uint TargetId { get; }

        public Packet RawPacket { get; }
    }
}