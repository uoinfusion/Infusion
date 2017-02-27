using System.Net.Sockets;

namespace Infusion.IO
{
    public class NetworkStreamToPullStreamAdapter : IPullStream
    {
        private readonly NetworkStream baseStream;

        public NetworkStreamToPullStreamAdapter(NetworkStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            baseStream.Dispose();
        }

        public bool DataAvailable => baseStream.DataAvailable;

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