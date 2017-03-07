using System;
using System.Text;
using Infusion.Packets;

namespace Infusion.IO
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
            array[Position++] = (byte) ((value >> 8) & 0xFF);
            array[Position++] = (byte) (value & 0xFF);
        }

        public void WriteByte(byte value)
        {
            array[Position++] = value;
        }

        public void WriteInt(int value)
        {
            array[Position++] = (byte) ((value >> 24) & 0xFF);
            array[Position++] = (byte) ((value >> 16) & 0xFF);
            array[Position++] = (byte) ((value >> 8) & 0xFF);
            array[Position++] = (byte) (value & 0xFF);
        }

        public void WriteUInt(uint value)
        {
            array[Position++] = (byte) ((value >> 24) & 0xFF);
            array[Position++] = (byte) ((value >> 16) & 0xFF);
            array[Position++] = (byte) ((value >> 8) & 0xFF);
            array[Position++] = (byte) (value & 0xFF);
        }

        public void WriteModelId(ModelId itemType)
        {
            WriteUShort((ushort) itemType);
        }

        public void WriteColor(Color color)
        {
            WriteUShort(color.Id);
        }

        public void WriteMovement(Movement movement)
        {
            WriteByte((byte)
                ((movement.Type == MovementType.Walk) ? (byte) movement.Direction : 0x80 + (byte) movement.Direction));
        }

        public void WriteString(string str)
        {
            WriteBytes(Encoding.ASCII.GetBytes(str));
        }

        private void WriteBytes(byte[] bytes)
        {
            foreach (byte b in bytes)
            {
                WriteByte(b);
            }
        }
    }
}