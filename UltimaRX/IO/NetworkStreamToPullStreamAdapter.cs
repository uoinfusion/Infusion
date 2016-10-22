using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.IO
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

        public bool DataAvailable => this.baseStream.DataAvailable;

        public int ReadByte()
        {
            return this.baseStream.ReadByte();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return this.baseStream.Read(buffer, offset, count);
        }
    }
}
