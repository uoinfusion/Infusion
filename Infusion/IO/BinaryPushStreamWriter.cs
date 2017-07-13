using System;

namespace Infusion.IO
{
    public sealed class BinaryPushStreamWriter : IDisposable
    {
        private readonly IPushStream baseStream;

        public BinaryPushStreamWriter(IPushStream baseStream)
        {
            this.baseStream = baseStream;
        }

        public void Dispose()
        {
            baseStream?.Dispose();
        }

        public void Flush()
        {
            baseStream.Flush();
        }

        public void Write(long value)
        {
            Write((byte) value);
            Write((byte) (value >> 8));
            Write((byte) (value >> 16));
            Write((byte) (value >> 24));
            Write((byte) (value >> 32));
            Write((byte) (value >> 40));
            Write((byte) (value >> 48));
            Write((byte) (value >> 56));
        }

        public void Write(ulong value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
            Write((byte)(value >> 16));
            Write((byte)(value >> 24));
            Write((byte)(value >> 32));
            Write((byte)(value >> 40));
            Write((byte)(value >> 48));
            Write((byte)(value >> 56));
        }

        public void Write(uint value)
        {
            Write((byte)value);
            Write((byte)(value >> 8));
            Write((byte)(value >> 16));
            Write((byte)(value >> 24));
        }

        public void Write(byte value)
        {
            baseStream.WriteByte(value);
        }

        public void Write(byte[] payload, int offset, int count)
        {
            baseStream.Write(payload, offset, count);
        }
    }
}