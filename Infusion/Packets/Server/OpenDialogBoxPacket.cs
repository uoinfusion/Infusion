using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class OpenDialogBoxPacket : MaterializedPacket
    {
        public uint DialogId { get; private set; }
        public ushort MenuId { get; private set; }
        public string Question { get; private set; }
        public DialogBoxResponse[] Responses { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);

            DialogId = reader.ReadUInt();
            MenuId = reader.ReadUShort();
            var questionLength = reader.ReadByte();
            Question = reader.ReadString(questionLength);
            var responsesCount = reader.ReadByte();

            if (responsesCount == byte.MaxValue)
                throw new PacketParsingException(rawPacket, $"Responses count is {responsesCount}. Cannot have byte 1 based index for all responses (last index would be 256 which cannot be encoded to a single byte).");

            Responses = new DialogBoxResponse[responsesCount];


            for (byte i = 0; i < responsesCount; i++)
            {
                var modelId = reader.ReadModelId();
                var color = reader.ReadColor();
                var responseTextLength = reader.ReadByte();
                var responseText = reader.ReadString(responseTextLength);

                Responses[i] = new DialogBoxResponse((byte)(i + 1), modelId, color, responseText);
            }
        }

        private Packet rawPacket;
        public override Packet RawPacket => rawPacket;
    }
}
