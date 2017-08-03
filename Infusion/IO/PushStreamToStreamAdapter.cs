using System;
using System.IO;

namespace Infusion.IO
{
    internal sealed class PushStreamToStreamAdapter : Stream
    {
        private readonly IPushStream baseStream;

        public PushStreamToStreamAdapter(IPushStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            baseStream.Write(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            baseStream.WriteByte(value);
        }
    }
}