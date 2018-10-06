using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class TalkRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public TalkRequest()
        {
        }

        public TalkRequest(SpeechType type, Color color, ushort speechFont, string message)
        {
            ushort length = (ushort)(message.Length + 8);
            byte[] payload = new byte[length];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.TalkRequest.Id);
            writer.WriteUShort(length);
            writer.WriteByte((byte)type);
            writer.WriteColor(color);
            writer.WriteUShort(speechFont);
            writer.WriteString(message);

            rawPacket = new Packet(PacketDefinitions.TalkRequest.Id, payload);
            Type = type;
            Color = color;
            SpeechFont = speechFont;
            Message = message;
        }

        public override Packet RawPacket => rawPacket;

        public SpeechType Type { get; set; }
        public Color Color { get; set; }
        public ushort SpeechFont { get; set; }
        public string Message { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            var length = reader.ReadUShort();

            Type = (SpeechType)reader.ReadByte();
            Color = reader.ReadColor();
            SpeechFont = reader.ReadUShort();
            Message = reader.ReadString(length - 8);
        }
    }
}
