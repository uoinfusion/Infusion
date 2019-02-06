namespace Infusion.IO.Encryption.Login
{
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
}