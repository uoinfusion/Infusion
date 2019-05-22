using System;
using System.IO;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class SpeechRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.ReadByte();
            reader.ReadUShort();
            Type = (SpeechType)reader.ReadByte();
            Color = reader.ReadUShort();
            Font = reader.ReadUShort();
            Language = reader.ReadString(4);
            if (((int)Type & 0xC0) != 0)
            {
                ushort rawBytes = reader.ReadUShort();
                int keywordCount = (rawBytes & 0x0FFF) >> 4;
                if (keywordCount > 0)
                {
                    reader.ReadByte();
                    keywordCount--;
                }
                while (keywordCount != 0)
                {
                    reader.ReadUShort();
                    keywordCount--;
                }

                Text = reader.ReadNullTerminatedString();
            }
            else
            {
                Text = reader.ReadNullTerminatedUnicodeString();
            }
        }

        public SpeechType Type { get; set; }

        public ushort Color { get; set; }

        public ushort Font { get; set; }

        public string Text { get; set; }

        public string Language { get; set; }

        public ushort[] Keywords { get; set; }

        public override Packet RawPacket
        {
            get
            {
                using (var stream = new MemoryStream())
                {
                    ushort textLength = (this.Text.Length < ushort.MaxValue) ? (ushort)this.Text.Length : ushort.MaxValue;
                    string text = (this.Text.Length < ushort.MaxValue) ? Text : Text.Substring(0, ushort.MaxValue);

                    var writer = new StreamPacketWriter(stream);
                    writer.WriteByte((byte)PacketDefinitions.SpeechRequest.Id);

                    int keywordBytes = (int)Math.Ceiling((Keywords.Length + 1) * 1.5f);
                    bool encoded = Keywords != null && Keywords.Length > 0;

                    if (encoded)
                    {
                        writer.WriteUShort((ushort)(13 + textLength + keywordBytes));
                        writer.WriteByte((byte)(Type | SpeechType.EncodedCommands));
                    }
                    else
                    {
                        writer.WriteUShort((ushort)(14 + textLength * 2));
                        writer.WriteByte((byte)Type);
                    }

                    writer.WriteUShort(Color);
                    writer.WriteUShort(Font);
                    writer.WriteNullTerminatedString(Language, 4);

                    if (encoded)
                    {
                        byte[] t = new byte[keywordBytes];
                        t[0] = (byte)((Keywords.Length & 0x0FF0) >> 4);
                        t[1] = (byte)((Keywords.Length & 0x000F) << 4);

                        for (int i = 0; i < Keywords.Length; i++)
                        {
                            int index = (int)((i + 1) * 1.5f);

                            if (i % 2 == 0)
                            {
                                t[index + 0] |= (byte)((Keywords[i] & 0x0F00) >> 8);
                                t[index + 1] = (byte)(Keywords[i] & 0x00FF);
                            }
                            else
                            {
                                t[index] = (byte)((Keywords[i] & 0x0FF0) >> 4);
                                t[index + 1] = (byte)((Keywords[i] & 0x000F) << 4);
                            }
                        }

                        for (int i = 0; i < t.Length; i++)
                            writer.WriteByte(t[i]);
                        writer.WriteNullTerminatedString(text);
                    }
                    else
                    {
                        writer.WriteNullTerminatedUnicodeString(text);
                    }

                    return new Packet(PacketDefinitions.SpeechRequest.Id, stream.ToArray());
                }
            }
        }
    }
}
