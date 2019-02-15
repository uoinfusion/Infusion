using Infusion.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Server
{
    internal sealed class CompressedGumpPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public GumpTypeId GumpTypeId { get; set; }
        public GumpInstanceId GumpId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Commands { get; set; }
        public string[] TextLines { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            var size = reader.ReadUShort();
            GumpId = (GumpInstanceId)reader.ReadUInt();
            GumpTypeId = (GumpTypeId)reader.ReadUInt();
            X = reader.ReadInt();
            Y = reader.ReadInt();
            int clen = reader.ReadInt() - 4;
            int dlen = (int)reader.ReadUInt();
            byte[] data = new byte[clen];
            reader.Read(data, 0, clen);
            byte[] decData = new byte[dlen];

            Decompress(data, 0, decData, dlen);
            Commands = Encoding.UTF8.GetString(decData);

            uint linesNum = reader.ReadUInt();

            if (linesNum > 0)
            {
                clen = reader.ReadInt() - 4;
                dlen = reader.ReadInt();

                data = new byte[clen];
                reader.Read(data, 0, clen);

                decData = new byte[dlen];
                Decompress(data, 0, decData, dlen);
                TextLines = new string[linesNum];

                for (int i = 0, index = 0; i < linesNum; i++)
                {
                    int length = (decData[index++] << 8) | decData[index++];
                    byte[] text = new byte[length * 2];
                    Buffer.BlockCopy(decData, index, text, 0, text.Length);
                    index += text.Length;
                    TextLines[i] = Encoding.BigEndianUnicode.GetString(text);
                }
            }
            else
            {
                TextLines = Array.Empty<string>();
            }
        }

        internal static void Decompress(byte[] source, int offset, byte[] dest, int length)
        {
            using (MemoryStream ms = new MemoryStream(source, offset, source.Length - offset))
            {
                ms.Seek(2, SeekOrigin.Begin);
                using (DeflateStream stream = new DeflateStream(ms, CompressionMode.Decompress)) stream.Read(dest, 0, length);
            }
        }
    }
}
