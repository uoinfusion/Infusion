using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class SpeechMessagePacket : MaterializedPacket
    {
        public uint Id { get; private set; }

        public ushort Model { get; private set; }

        public SpeechType Type { get; private set; }

        public Color Color { get; private set; }

        public ushort Font { get; private set; }

        public string Name { get; private set; }

        public string Message { get; private set; }

        public string Language { get; private set; }

        private Packet rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Position = 3;

            Id = reader.ReadUInt();
            Model = reader.ReadUShort();
            Type = (SpeechType)reader.ReadByte();
            Color = (Color) reader.ReadUShort();
            Font = reader.ReadUShort();
            Language = reader.ReadString(4);
            Name = reader.ReadString(30);
            Message = reader.ReadNullTerminatedUnicodeString();
        }

        public override Packet RawPacket => this.rawPacket;
    }
}
