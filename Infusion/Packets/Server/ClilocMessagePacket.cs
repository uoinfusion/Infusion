using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public sealed class ClilocMessagePacket : MaterializedPacket
    {
        public ObjectId SpeakerId { get; set; }
        public ModelId SpeakerBody { get; set; }
        public Color Color { get; set; }
        public ushort Font { get; set; }
        public MessageId MessageId { get; set; }
        public string Name { get; set; }
        public string Arguments { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            var packetSize = reader.ReadUShort();
            SpeakerId = reader.ReadObjectId();
            SpeakerBody = reader.ReadModelId();
            reader.ReadByte(); // type
            Color = reader.ReadColor();
            Font = reader.ReadUShort();
            MessageId = new MessageId(reader.ReadInt());
            Name = reader.ReadString(30);
            Arguments = reader.ReadNullTerminatedUnicodeString();
        }

        public override Packet RawPacket => rawPacket;

        private Packet rawPacket;
    }
}
