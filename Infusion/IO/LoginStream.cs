using System;
using System.IO;

namespace Infusion.IO
{
    internal sealed class LoginPullStream : IPullStream
    {
        private readonly bool encrypted;
        private LoginCrypt loginCrypt;
        private readonly byte[] output = new byte[1];
        private readonly byte[] input = new byte[1];

        public IPullStream BaseStream { get; set; }

        public LoginPullStream(bool encrypted)
        {
            this.encrypted = encrypted;
        }

        public void SetSeed(uint seed)
        {
            loginCrypt = new LoginCrypt(seed);
        }

        public bool DataAvailable => BaseStream?.DataAvailable ?? false;

        public void Dispose() => BaseStream?.Dispose();
        public int Read(byte[] buffer, int offset, int count)
        {
            if (encrypted && loginCrypt != null)
            {
                for (var i = 0; i < count; i++)
                {
                    input[0] = (byte)BaseStream.ReadByte();
                    loginCrypt.Encrypt(input, output, 1);
                    buffer[i + offset] = output[0];
                }

                return count;
            }
            else
                return BaseStream.Read(buffer, offset, count);
        }

        public int ReadByte()
        {
            if (encrypted && loginCrypt != null)
            {
                input[0] = (byte)BaseStream.ReadByte();
                loginCrypt.Encrypt(input, output, 1);
                return output[0];
            }
            else
                return BaseStream.ReadByte();
        }
    }

    internal sealed class LoginPushStream : IPushStream
    {
        private readonly byte[] output = new byte[1];
        private readonly byte[] input = new byte[1];
        private readonly bool encrypted;
        private LoginCrypt loginCrypt;

        public LoginPushStream(bool encrypted)
        {
            this.encrypted = encrypted;
        }

        public void SetSeed(uint seed)
        {
            loginCrypt = new LoginCrypt(seed);
        }

        public IPushStream BaseStream { get; set; }

        public void Dispose() => BaseStream?.Dispose();
        public void Flush() => BaseStream?.Flush();
        public void Write(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
                WriteByte(buffer[i + offset]);
        }

        public void WriteByte(byte value)
        {
            if (encrypted && loginCrypt != null)
            {
                input[0] = value;
                loginCrypt.Encrypt(input, output, 1);

                BaseStream.WriteByte(output[0]);
            }
            else
                BaseStream.WriteByte(value);
        }
    }

    internal sealed class LoginCrypt
    {
        private readonly uint[] key;
        private readonly uint key1;
        private readonly uint key2;

        public LoginCrypt(uint seed)
        {
            uint k1 = 0x2cc3ed9d;
            var k2 = 0xa374227f;

            key = new uint[2];

            key[0] =
                ((~seed ^ 0x00001357) << 16)
                | ((seed ^ 0xffffaaaa) & 0x0000ffff);
            key[1] =
                ((seed ^ 0x43210000) >> 16)
                | ((~seed ^ 0xabcdffff) & 0xffff0000);

            key1 = k1;
            key2 = k2;
        }

        public void Encrypt(byte[] input, byte[] output, long len)
        {
            for (var i = 0; i < len; i++)
            {
                output[i] = (byte)(input[i] ^ (byte)key[0]);

                var table0 = key[0];
                var table1 = key[1];

                key[1] = (((((table1 >> 1) | (table0 << 31)) ^ key1) >> 1)
                            | (table0 << 31)) ^ key1;
                key[0] = ((table0 >> 1) | (table1 << 31)) ^ key2;
            }
        }

        public void Decrypt(byte[] input, byte[] output, long len)
            => Encrypt(input, output, len);
    }
}