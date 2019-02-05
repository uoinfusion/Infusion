using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.IO
{
    internal struct LoginEncryptionKey
    {
        public uint Key1 { get; }
        public uint Key2 { get; }

        public LoginEncryptionKey(uint key1, uint key2)
        {
            Key1 = key1;
            Key2 = key2;
        }
    }

    internal sealed class LoginEncryptionDetectionResult
    {
        public byte[] DecryptedPacket { get; }
        public LoginEncryptionKey? Key { get; }

        public bool IsEncrypted => Key.HasValue;

        public LoginEncryptionDetectionResult(byte[] decryptedPacket, LoginEncryptionKey encryption)
        {
            DecryptedPacket = decryptedPacket;
            Key = encryption;
        }

        public LoginEncryptionDetectionResult(byte[] decryptedPacket)
        {
            DecryptedPacket = decryptedPacket;
            Key = null;
        }
    }

    internal sealed class LoginEncryptionDetector
    {
        private readonly byte[] rawBuffer = new byte[62];
        private readonly byte[] decryptedBuffer = new byte[62];

        private readonly LoginEncryptionKey[] encryptionKeys =
        {
            new LoginEncryptionKey(0x2cc3ed9d, 0xa374227f)
        };

        public LoginEncryptionDetectionResult Detect(uint seed, IPullStream inputStream)
        {
            var reader = new StreamPacketReader(new PullStreamToStreamAdapter(inputStream), rawBuffer);
            int length = 0;
            while (inputStream.DataAvailable && length < 62)
                rawBuffer[length++] = reader.ReadByte();

            rawBuffer.CopyTo(decryptedBuffer, 0);

            int i = -1;
            while (!IsGameServerPacket(decryptedBuffer))
            {
                i++;
                if (i >= encryptionKeys.Length)
                    throw new InvalidOperationException("Encryption not recognized.");

                var loginCrypt = new LoginCrypt(seed, encryptionKeys[i]);
                loginCrypt.Decrypt(rawBuffer, decryptedBuffer, 62);
            }

            if (i >= 0)
                return new LoginEncryptionDetectionResult(decryptedBuffer, encryptionKeys[i]);
            else
                return new LoginEncryptionDetectionResult(decryptedBuffer);
        }

        private bool IsGameServerPacket(byte[] encryptedBuffer)
        {
            if (encryptedBuffer[0] != 0x80)
                return false;

            return true;
        }

    }
}
