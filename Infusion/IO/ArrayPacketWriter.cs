using System;
using System.Text;
using Infusion.Packets;

namespace Infusion.IO
{
    internal sealed class ArrayPacketWriter
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

        public void WriteSByte(sbyte value)
        {
            array[Position++] = (byte)value;
        }

        public void WriteInt(int value)
        {
            array[Position++] = (byte) ((value >> 24) & 0xFF);
            array[Position++] = (byte) ((value >> 16) & 0xFF);
            array[Position++] = (byte) ((value >> 8) & 0xFF);
            array[Position++] = (byte) (value & 0xFF);
        }

        public void WriteId(ObjectId value) => WriteUInt(value.Value);

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

        public void WriteMovement(Direction direction, MovementType movementType)
        {
            WriteByte((byte)
                ((movementType == MovementType.Walk) ? (byte) direction : 0x80 + (byte) direction));
        }

        public void WriteString(string str)
        {
            WriteBytes(Encoding.ASCII.GetBytes(str));
        }

        public void WriteUnicodeString(string str)
        {
            foreach (char ch in str)
                WriteUShort(ch);
        }

        public void WriteString(int length, string str)
        {
            WriteBytes(Encoding.ASCII.GetBytes(str.ToCharArray(), 0, str.Length));

            if (str.Length < length)
            {
                while (str.Length < length)
                {
                    WriteByte(0x00);
                    length--;
                }
            }
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