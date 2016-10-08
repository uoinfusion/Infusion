namespace UltimaRX.Packets
{
    internal class ArrayPacketReader : IPacketReader
    {
        private readonly byte[] array;
        private int position;

        public ArrayPacketReader(byte[] array, int position)
        {
            this.array = array;
            this.position = position;
        }

        public byte ReadByte()
        {
            return this.array[position++];
        }

        public ushort ReadUShort()
        {
            return (ushort)((array[position++] << 8) + array[position++]);
        }
    }
}