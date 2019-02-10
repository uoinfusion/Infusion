using Infusion.IO;
using System.Linq;

namespace Infusion.Packets.Server
{
    internal sealed class SendGumpMenuDialogPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public GumpTypeId GumpTypeId { get; set; }
        public GumpInstanceId GumpId { get; set; }
        public uint X { get; set; }
        public uint Y { get; set; }
        public string Commands { get; set; }
        public string[] TextLines { get; set; }

        public override Packet RawPacket => rawPacket;

        public Packet Serialize()
        {
            int packetLength = 23 + Commands.Length + (TextLines?.Sum(x => (x.Length * 2) + 2) ?? 0);

            var payload = new byte[packetLength];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte(0xB0);
            writer.WriteUShort((ushort)packetLength);
            writer.WriteUInt((uint)GumpId);
            writer.WriteUInt((uint)GumpTypeId);
            writer.WriteUInt(X);
            writer.WriteUInt(Y);
            writer.WriteUShort((ushort)Commands.Length);
            writer.WriteString(Commands);
            writer.WriteUShort((ushort)(TextLines?.Length ?? 0));

            if (TextLines != null)
            {
                foreach (var line in TextLines)
                {
                    writer.WriteUShort((ushort)line.Length);
                    writer.WriteUnicodeString(line);
                }
            }

            return new Packet(0xB0, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            GumpId = new GumpInstanceId(reader.ReadUInt());
            GumpTypeId = new GumpTypeId(reader.ReadUInt());
            X = reader.ReadUInt();
            Y = reader.ReadUInt();

            var commandSectionLength = reader.ReadUShort();
            Commands = reader.ReadString(commandSectionLength);

            var textLinesCount = reader.ReadUShort();
            TextLines = new string[textLinesCount];
            for (var i = 0; i < textLinesCount; i++)
            {
                var textLength = reader.ReadUShort();
                TextLines[i] = reader.ReadUnicodeString(textLength);
            }
        }
    }
}