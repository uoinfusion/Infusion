using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class SendSpeechPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId Id { get; set; }

        public ModelId Model { get; set; }

        public SpeechType Type { get; set; }

        public Color Color { get; set; }

        public ushort Font { get; set; }

        public string Name { get; set; }

        public string Message { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            int size = reader.ReadUShort();

            Id = reader.ReadObjectId();
            Model = reader.ReadModelId();
            Type = (SpeechType) reader.ReadByte();
            Color = (Color) reader.ReadUShort();
            Font = reader.ReadUShort();

            Name = reader.ReadNullTerminatedString();
            if (size > 44)
                reader.Position = 44;
            Message = reader.ReadNullTerminatedString();
        }

        public void Serialize()
        {
            ushort size = (ushort)(45 + Message.Length);

            byte[] payload = new byte[size];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.SendSpeech.Id);
            writer.WriteUShort(size);
            writer.WriteId(Id);
            writer.WriteModelId(Model);
            writer.WriteByte((byte)Type);
            writer.WriteUShort(Color.Id);
            writer.WriteUShort(Font);
            writer.WriteString(30, Name);
            writer.WriteString(Message);
            writer.WriteByte(0x00);

            rawPacket = new Packet(PacketDefinitions.SendSpeech.Id, payload);
        }

        public override Packet RawPacket => rawPacket;
    }
}
