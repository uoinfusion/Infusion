using System.IO;

namespace UltimaRX.IO
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
    }
}