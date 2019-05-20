using System.IO;
using System.Text;

namespace Infusion.IO
{
    internal sealed class StreamPacketWriter
    {
        private readonly Stream stream;

        public StreamPacketWriter(Stream stream)
        {
            this.stream = stream;
        }

        public int Length { get; set; }

        public void WriteByte(byte v)
        {
            stream.WriteByte(v);
            Length++;
        }

        public void WriteUShort(ushort v)
        {
            WriteByte((byte) (v >> 8));
            WriteByte((byte) v);
        }

        public void WriteBytes(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                WriteByte(b);
            }
        }

        internal void WriteString(string str, int maximalLength)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            WriteBytes(bytes);

            for (var i = 0; str.Length + i < maximalLength; i++)
            {
                WriteByte(0x00);
            }
        }

        internal void WriteNullTerminatedString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            WriteBytes(bytes);
            WriteByte(0x00);
        }

        public void WriteNullTerminatedString(string str, int maximalLength)
        {
            WriteString(str + "\0", maximalLength);
        }

        public void WriteUnicodeString(string str)
        {
            for (var i = 0; i < str.Length; i++)
            {
                WriteUShort(str[i]);
            }
        }

        public void WriteNullTerminatedUnicodeString(string str)
        {
            WriteUnicodeString(str);
            WriteUShort(0);
        }
    }
}