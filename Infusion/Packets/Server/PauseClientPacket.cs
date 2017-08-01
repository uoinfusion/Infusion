using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class PauseClientPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public PauseClientChoice Choice { get; set; }

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