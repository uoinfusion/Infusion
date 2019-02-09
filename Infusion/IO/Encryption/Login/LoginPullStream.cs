namespace Infusion.IO.Encryption.Login
{
    internal sealed class LoginPullStream : IPullStream
    {
        private readonly LoginCrypt loginCrypt;
        private readonly byte[] output = new byte[1];
        private readonly byte[] input = new byte[1];

        public IPullStream BaseStream { get; set; }

        public LoginPullStream() : this(0, null)
        {
        }

        public LoginPullStream(LoginCrypt loginCrypt)
        {
            this.loginCrypt = loginCrypt;
        }

        public LoginPullStream(uint seed, LoginEncryptionKey? key)
        {
            if (key.HasValue)
                loginCrypt = new LoginCrypt(seed, key.Value);
        }

        public bool DataAvailable => BaseStream?.DataAvailable ?? false;

        public void Dispose() => BaseStream?.Dispose();
        public int Read(byte[] buffer, int offset, int count)
        {
            if (loginCrypt != null)
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
            if (loginCrypt != null)
            {
                input[0] = (byte)BaseStream.ReadByte();
                loginCrypt.Encrypt(input, output, 1);
                return output[0];
            }
            else
                return BaseStream.ReadByte();
        }
    }
}