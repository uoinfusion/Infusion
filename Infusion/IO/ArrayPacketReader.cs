using System;
using System.Text;
using Infusion.Packets;

namespace Infusion.IO
{
    public class ArrayPacketReader : IPacketReader
    {
        private readonly byte[] array;

        public ArrayPacketReader(byte[] array)
        {
            this.array = array;
        }

        public int Position { get; private set; }

        public byte ReadByte()
        {
            return array[Position++];
        }

        public ushort ReadUShort()
        {
            var result = ReadUShort(array, Position);
            Position += 2;
            return result;
        }

        public static ObjectId ReadId(byte[] array, int position) => new ObjectId(ReadUInt(array, position));
        public static ModelId ReadModelId(byte[] array, int position) => ReadUShort(array, position);

        public static (Direction, MovementType) ReadDirection(byte[] array, int position) =>
            ConvertToDirection(array[position]);

        public ModelId ReadModelId() => ReadUShort();

        internal static ushort ReadUShort(byte[] array, int position)
        {
            return (ushort) ((array[position++] << 8) + array[position]);
        }

        internal void Read(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
                buffer[offset + i] = ReadByte();
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

                if (charRead != 0)
                    s += (char) charRead;
            }
            return s;
        }

        public string ReadString(int length)
        {
            var str = new StringBuilder();
            bool nullReached = false;
            while (length > 0)
            {
                var ch = ReadByte();
                if (!nullReached)
                {
                    if (ch == '\0')
                        nullReached = true;
                    else
                        str.Append(value: (char) ch);
                }

                length--;
            }

            return str.ToString().TrimEnd('\0');
        }

        public static int ReadInt(byte[] array, int position)
        {
            return (array[position++] << 24) + (array[position++] << 16) + (array[position++] << 8) + array[position];
        }

        public uint ReadUInt()
        {
            var result = ReadUInt(array, Position);

            Position += 4;

            return result;
        }

        public int ReadInt()
        {
            var result = ReadInt(array, Position);

            Position += 4;

            return result;
        }

        public static uint ReadUInt(byte[] array, int position)
        {
            return (uint) (array[position++] << 24) + (uint) (array[position++] << 16) + (uint) (array[position++] << 8) +
                   array[position];
        }

        public string ReadNullTerminatedString()
        {
            var builder = new StringBuilder();
            var charRead = ReadByte();
            while (charRead != 0)
            {
                builder.Append((char) charRead);
                charRead = ReadByte();
            }
            return builder.ToString();
        }

        public void Skip(int numberOfBytes)
        {
            Position += numberOfBytes;
        }

        public Color ReadColor() => (Color) ReadUShort();
        public Layer ReadLayer() => (Layer) ReadByte();

        public string ReadUnicodeString(int count)
        {
            StringBuilder str = new StringBuilder();
            bool append = true;
            while (count > 0)
            {
                ushort ch = ReadUShort();
                if (ch == 0)
                {
                    append = false;
                }
                if (append && IsSafeChar(ch))
                {
                    str.Append((char)ch);
                }
                count--;
            }
            return str.ToString();
        }

        public static bool IsSafeChar(ushort ch)
        {
            if (ch >= 0x20)
            {
                return (ch < 0xfffe);
            }
            return false;
        }

        public ObjectId ReadObjectId() => new ObjectId(ReadUInt());

        public (Direction, MovementType) ReadDirection() => ConvertToDirection(ReadByte());

        private static (Direction, MovementType) ConvertToDirection(byte rawDirection)
        {
            Direction direction;
            MovementType type;

            if ((rawDirection & 0x80) != 0)
            {
                direction = (Direction)(rawDirection - 0x80);
                type = MovementType.Run;
            }
            else
            {
                direction = (Direction)rawDirection;
                type = MovementType.Walk;
            }

            return (direction, type);
        }
    }
}