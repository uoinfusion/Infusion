using Infusion.IO;

namespace Infusion.Packets.Both
{
    public class RequestWarMode
    {
        public RequestWarMode(WarMode flag)
        {
            Flag = flag;

            var payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.RequestWarMode.Id);
            writer.WriteByte((byte) flag);
            writer.WriteByte(0x00);
            writer.WriteByte(0x32);
            writer.WriteByte(0x00);

            RawPacket = new Packet(PacketDefinitions.RequestWarMode.Id, payload);
        }

        public WarMode Flag { get; }
        public Packet RawPacket { get; }
    }
}