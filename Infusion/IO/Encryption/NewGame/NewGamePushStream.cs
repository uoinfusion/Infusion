using System;

namespace Infusion.IO.Encryption.NewGame
{
    internal abstract class NewGamePushStream : IPushStream
    {
        private readonly byte[] encrypted = new byte[1];
        private readonly byte[] tmp = new byte[1];
        private readonly Action<byte[], byte[], long> encrypt;

        public IPushStream BaseStream { get; set; }

        public NewGamePushStream(Action<byte[], byte[], long> encrypt)
        {
            this.encrypt = encrypt;
        }

        public void Dispose() => BaseStream?.Dispose();
        public void Flush() => BaseStream?.Flush();

        public void Write(byte[] buffer, int offset, int count)
        {
            if (encrypt != null)
            {
                for (var i = offset; i < count; i++)
                {
                    tmp[0] = buffer[i];
                    encrypt(tmp, encrypted, 1);

                    BaseStream.Write(encrypted, 0, 1);
                }
            }
            else
            {
                BaseStream.Write(buffer, offset, count);
            }
        }

        public void WriteByte(byte value)
        {
            if (encrypt != null)
            {
                tmp[0] = value;
                encrypt(tmp, encrypted, 1);

                BaseStream.Write(encrypted, 0, 1);
            }
            else
                BaseStream.WriteByte(value);
        }
    }
}