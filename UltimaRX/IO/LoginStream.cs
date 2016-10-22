using System;
using System.IO;

namespace UltimaRX.IO
{
    public class LoginStream : Stream
    {
        private static readonly uint[] m_key;
        private static readonly uint m_key1;
        private static readonly uint m_key2;

        static LoginStream()
        {
            var seed = 0xA9FE5050;
            uint k1 = 0x2cc3ed9d;
            var k2 = 0xa374227f;

            m_key = new uint[2];

            m_key[0] =
                ((~seed ^ 0x00001357) << 16)
                | ((seed ^ 0xffffaaaa) & 0x0000ffff);
            m_key[1] =
                ((seed ^ 0x43210000) >> 16)
                | ((~seed ^ 0xabcdffff) & 0xffff0000);

            m_key1 = k1;
            m_key2 = k2;
        }

        public LoginStream(Stream baseStream)
        {
            BaseStream = baseStream;
        }

        public Stream BaseStream { get; set; }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => BaseStream.Length;

        public override long Position
        {
            get { return BaseStream.Position; }
            set { throw new NotSupportedException(); }
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return BaseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var output = new byte[1];
            var input = new byte[1];

            for (var i = 0; i < count; i++)
            {
                input[0] = buffer[i + offset];

                Encrypt(input, output, 1);

                BaseStream.WriteByte(output[0]);
            }
        }

        private void Encrypt(byte[] input, byte[] output, long len)
        {
            for (var i = 0; i < len; i++)
            {
                output[i] = (byte) (input[i] ^ (byte) m_key[0]);

                var table0 = m_key[0];
                var table1 = m_key[1];

                m_key[1] = (((((table1 >> 1) | (table0 << 31)) ^ m_key1) >> 1)
                            | (table0 << 31)) ^ m_key1;
                m_key[0] = ((table0 >> 1) | (table1 << 31)) ^ m_key2;
            }
        }

        private void Decrypt(byte[] input, byte[] output, long len)
        {
            for (var i = 0; i < len; i++)
                output[i] = input[i];
        }
    }
}