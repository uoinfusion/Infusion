using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class SendGumpMenuDialogPacket : MaterializedPacket
    {
        public uint Id { get; private set; }
        public uint GumpId { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public string Commands { get; private set; }
        public string[] TextLines { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader =new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            Id = reader.ReadUInt();
            GumpId = reader.ReadUInt();
            X = reader.ReadUInt();
            Y = reader.ReadUInt();

            ushort commandSectionLength = reader.ReadUShort();
            Commands = reader.ReadString(commandSectionLength);

            ushort textLinesCount = reader.ReadUShort();
            TextLines = new string[textLinesCount];
            for (int i = 0; i < textLinesCount; i++)
            {
                ushort textLength = reader.ReadUShort();
                TextLines[i] = reader.ReadUnicodeString(textLength);
            }
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
