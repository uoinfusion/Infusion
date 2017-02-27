using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class SendSpeechPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public uint Id { get; private set; }

        public ModelId Model { get; private set; }

        public SpeechType Type { get; private set; }

        public Color Color { get; private set; }

        public ushort Font { get; private set; }

        public string Name { get; private set; }

        public string Message { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            Id = reader.ReadUInt();
            Model = reader.ReadModelId();
            Type = (SpeechType) reader.ReadByte();
            Color = (Color) reader.ReadUShort();
            Font = reader.ReadUShort();
            Name = reader.ReadString(30);
            Message = reader.ReadNullTerminatedString();
        }

        public override Packet RawPacket => rawPacket;
    }
}
