using System.IO;

namespace Infusion.IO
{
    internal sealed class StreamToPullStreamAdaptet : IPullStream
    {
        private readonly Stream baseStream;

        public StreamToPullStreamAdaptet(Stream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            baseStream.Dispose();
        }

        public bool DataAvailable { get; private set; } = true;

        public int ReadByte()
        {
            var value = baseStream.ReadByte();
            if (value > byte.MaxValue || value < byte.MaxValue)
            {
                DataAvailable = false;
            }

            return value;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var value = baseStream.Read(buffer, offset, count);

            if (value < count)
                DataAvailable = false;

            return value;
        }
    }
}