using System.IO;

namespace Infusion.IO
{
    public class StreamToPushStreamAdapter : IPushStream
    {
        private readonly Stream baseStream;

        public StreamToPushStreamAdapter(Stream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            baseStream.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            baseStream.WriteByte(value);
        }

        public void Flush()
        {
            baseStream.Flush();
        }
    }
}