using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class PauseClientPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public PauseClientChoice Choice { get; private set; }

        public PauseClientPacket()
        {
        }

        public PauseClientPacket(PauseClientChoice choice)
        {
            Choice = choice;
            var payload = new byte[2];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.PauseClient.Id);
            writer.WriteByte((byte)choice);

            rawPacket = new Packet(payload[0], payload);
        }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            Choice = (PauseClientChoice) reader.ReadByte();
        }
    }
}