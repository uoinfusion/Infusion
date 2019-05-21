using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    internal sealed class SendChatMessageRequest
    {
        public string Message { get; }
        public string Language { get; }

        public Packet RawPacket { get; }

        public SendChatMessageRequest(string message, string language)
        {
            Message = message;
            Language = language;

            ushort length = (ushort)(11 + message.Length * 2);

            byte[] payload = new byte[length];
            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.ChatText.Id);
            writer.WriteUShort(length);
            writer.WriteString(4, language);
            writer.WriteUShort(0x61);
            writer.WriteUnicodeString(message);
            writer.WriteUShort(0);

            RawPacket = new Packet(PacketDefinitions.ChatText.Id, payload);
        }
    }
}
