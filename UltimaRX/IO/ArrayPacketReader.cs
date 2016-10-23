namespace UltimaRX.IO
{
    internal class ArrayPacketReader : IPacketReader
    {
        private readonly byte[] array;
        public int Position { get; set; }

        public ArrayPacketReader(byte[] array)
        {
            this.array = array;
            this.Position = Position;
        }

        public byte ReadByte()
        {
            return this.array[Position++];
        }

        internal void Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                buffer[offset + i] = ReadByte();
            }
        }

        public ushort ReadUShort()
        {
            return (ushort)((array[Position++] << 8) + array[Position++]);
        }
    }
}