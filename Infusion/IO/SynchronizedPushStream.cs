using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.IO
{
    internal sealed class SynchronizedPushStream : IPushStream
    {
        private readonly IPushStream baseStream;
        private readonly object sync = new object();

        public SynchronizedPushStream(IPushStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            lock (sync)
                baseStream.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            lock (sync)
                baseStream.Write(buffer, offset, count);
        }

        public void WriteByte(byte value)
        {
            lock (sync)
                baseStream.WriteByte(value);
        }

        public void Flush()
        {
            lock (sync)
                baseStream.Flush();
        }
    }
}
