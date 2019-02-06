namespace Infusion.IO.Encryption.Login
{
    internal sealed class LoginPushStream : IPushStream
    {
        private readonly byte[] output = new byte[1];
        private readonly byte[] input = new byte[1];
        private readonly LoginCrypt loginCrypt;

        public IPushStream BaseStream { get; set; }

        public LoginPushStream(uint seed, LoginEncryptionKey key)
        {
            loginCrypt = new LoginCrypt(seed, key);
        }

        public LoginPushStream()
        {
        }

        public void Dispose() => BaseStream?.Dispose();
        public void Flush() => BaseStream?.Flush();

        public void Write(byte[] buffer, int offset, int count)
        {
            for (var i = 0; i < count; i++)
                WriteByte(buffer[i + offset]);
        }

        public void WriteByte(byte value)
        {
            if (loginCrypt != null)
            {
                input[0] = value;
                loginCrypt.Encrypt(input, output, 1);

                BaseStream.WriteByte(output[0]);
            }
            else
                BaseStream.WriteByte(value);
        }
    }
}