using System.IO;

namespace Infusion.IO
{
    public class StreamPacketReader : IPacketReader
    {
        private readonly Stream sourceStream;
        private readonly byte[] targetBuffer;

        public StreamPacketReader(Stream sourceStream, byte[] targetBuffer)
        {
            this.sourceStream = sourceStream;
            this.targetBuffer = targetBuffer;
        }

        public int Position { get; private set; }

        public byte ReadByte()
        {
            var result = sourceStream.ReadByte();

            if ((result < byte.MinValue) || (result > byte.MaxValue))
                throw new EndOfStreamException();

            var resultByte = (byte) result;
            targetBuffer[Position++] = resultByte;

            return resultByte;
        }

        public ushort ReadUShort()
        {
            return (ushort) ((ReadByte() << 8) + ReadByte());
        }

        public void ReadBytes(int count)
        {
            for (var i = 0; i < count; i++)
                ReadByte();
        }
    }
}