using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class UpdateCurrentManaPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public uint PlayerId { get; private set; }

        public ushort MaxMana { get; private set; }

        public ushort CurrentMana { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            MaxMana = reader.ReadUShort();
            CurrentMana = reader.ReadUShort();
        }
    }
}