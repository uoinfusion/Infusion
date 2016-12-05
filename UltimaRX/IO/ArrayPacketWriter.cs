using System;

namespace UltimaRX.IO
{
    public class ArrayPacketWriter
    {
        private readonly byte[] array;

        public ArrayPacketWriter(byte[] array)
        {
            this.array = array;
        }

        public int Position { get; set; }

        public void Write(byte[] buffer, int offset, int count)
        {
            Array.Copy(buffer, offset, array, Position, count);
            Position += count;
        }

        internal void WriteUShort(ushort value)
        {
            array[Position++] = (byte)((value >> 8) & 0xFF);
            array[Position++] = (byte) (value & 0xFF);

        }

        public void WriteByte(byte value)
        {
            array[Position++] = value;
        }

        public void WriteInt(int value)
        {
            array[Position++] = (byte)((value >> 24) & 0xFF);
            array[Position++] = (byte)((value >> 16) & 0xFF);
            array[Position++] = (byte)((value >> 8) & 0xFF);
            array[Position++] = (byte)(value & 0xFF);
        }
    }
}