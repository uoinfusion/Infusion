using System.IO;
using UltimaRX.Packets;

namespace UltimaRX.IO
{
    public class StreamPacketReader : IPacketReader
    {
        private readonly Stream sourceStream;
        private readonly byte[] targetBuffer;

        public int Position { get; private set; }

        public StreamPacketReader(Stream sourceStream, byte[] targetBuffer)
        {
            this.sourceStream = sourceStream;
            this.targetBuffer = targetBuffer;
        }

        public byte ReadByte()
        {
            int result = sourceStream.ReadByte();

            if (result < byte.MinValue || result > byte.MaxValue)
            {
                throw new EndOfStreamException();
            }

            byte resultByte = (byte) result;
            targetBuffer[Position++] = resultByte;

            return resultByte;
        }

        public ushort ReadUShort()
        {
            return (ushort)((ReadByte() << 8) + ReadByte());
        }

        public void ReadBytes(int count)
        {
            for (int i = 0; i < count; i++)
            {
                ReadByte();
            }
        }
    }
}