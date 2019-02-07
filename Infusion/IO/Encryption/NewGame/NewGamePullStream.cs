using System;
using System.IO;

namespace Infusion.IO.Encryption.NewGame
{
    internal abstract class NewGamePullStream : IPullStream
    {
        protected Action<byte[], byte[], long> encrypt;

        public IPullStream BaseStream { get; set; }

        public bool DataAvailable => BaseStream?.DataAvailable ?? false;

        public void Dispose() => BaseStream?.Dispose();

        public int Read(byte[] buffer, int offset, int count)
        {
            if (encrypt != null)
            {
                var encrypted = new byte[count + 1];
                var encryptedCount = BaseStream.Read(encrypted, 0, count);

                encrypt(encrypted, buffer, count);

                return encryptedCount;
            }
            else
            {
                return BaseStream.Read(buffer, offset, count);
            }
        }

        public int ReadByte()
        {
            if (encrypt != null)
            {
                var encrypted = new byte[1];
                var value = BaseStream.ReadByte();
                if ((value < 0) || (value > 255))
                    throw new EndOfStreamException();

                encrypted[0] = (byte)value;

                var buffer = new byte[1];
                encrypt(encrypted, buffer, 1);

                return buffer[0];
            }
            else
            {
                return BaseStream.ReadByte();
            }
        }
    }
}