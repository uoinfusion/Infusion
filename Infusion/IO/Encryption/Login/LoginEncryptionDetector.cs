using System;

namespace Infusion.IO.Encryption.Login
{
    internal sealed class LoginEncryptionDetector
    {
        private readonly byte[] rawBuffer = new byte[62];
        private readonly byte[] decryptedBuffer = new byte[62];

        public LoginEncryptionDetectionResult Detect(uint seed, IPullStream inputStream)
        {
            var reader = new StreamPacketReader(new PullStreamToStreamAdapter(inputStream), rawBuffer);
            var length = 0;
            while (inputStream.DataAvailable && length < 62)
                rawBuffer[length++] = reader.ReadByte();

            rawBuffer.CopyTo(decryptedBuffer, 0);

            var i = -1;
            while (!IsGameServerPacket(decryptedBuffer))
            {
                i++;
                if (i >= LoginEncryptionKey.Keys.Length)
                    throw new InvalidOperationException("Encryption not recognized.");

                var loginCrypt = new LoginCrypt(seed, LoginEncryptionKey.Keys[i]);
                loginCrypt.Decrypt(rawBuffer, decryptedBuffer, 62);
            }

            if (i >= 0)
                return new LoginEncryptionDetectionResult(decryptedBuffer, LoginEncryptionKey.Keys[i]);
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
