using System.IO;

namespace Infusion.IO
{
    public class MemoryStreamToPullStreamAdapter : IPullStream
    {
        private readonly MemoryStream baseStream;

        public MemoryStreamToPullStreamAdapter(MemoryStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            baseStream.Dispose();
        }

        public bool DataAvailable => baseStream.Position < baseStream.Length;

        public int ReadByte()
        {
            return baseStream.ReadByte();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return baseStream.Read(buffer, offset, count);
        }
    }
}