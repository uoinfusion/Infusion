using System.Text;

namespace UltimaRX.IO
{
    internal class ArrayPacketReader : IPacketReader
    {
        private readonly byte[] array;

        public ArrayPacketReader(byte[] array)
        {
            this.array = array;
            Position = Position;
        }

        public int Position { get; set; }

        public byte ReadByte()
        {
            return array[Position++];
        }

        public ushort ReadUShort()
        {
            return (ushort) ((array[Position++] << 8) + array[Position++]);
        }

        internal void Read(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                buffer[offset + i] = ReadByte();
            }
        }

        public short ReadShort()
        {
            return (short) ((ReadByte() << 8) + ReadByte());
        }

        public string ReadNullTerminatedUnicodeString()
        {
            var s = "";
            short charRead = -1;
            while (charRead != 0)
            {
                charRead = ReadShort();

                if (charRead == 0)
                {
                    ReadShort();
                }
                else
                {
                    s += (char) charRead;
                }
            }
            return s;
        }

        public string ReadString(int length)
        {
            var str = new StringBuilder();
            while (length > 0)
            {
                var ch = ReadByte();
                str.Append((char) ch);
                length--;
            }

            return str.ToString();
        }
    }
}