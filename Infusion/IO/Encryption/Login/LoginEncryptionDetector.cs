using System;

namespace Infusion.IO.Encryption.Login
{
    internal sealed class LoginEncryptionDetector
    {
        private Version[] supportedVersions =
        {
            new Version(7,0,74),
            new Version(3,0,6),
        };

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
            LoginEncryptionKey? currentKey = null;
            LoginCrypt loginCrypt = null;
            while (!IsGameServerPacket(decryptedBuffer))
            {
                i++;
                if (i >= supportedVersions.Length)
                    throw new InvalidOperationException("Encryption not recognized.");

                currentKey = CalculateKey(supportedVersions[i]);
                loginCrypt = new LoginCrypt(seed, currentKey.Value);
                loginCrypt.Decrypt(rawBuffer, decryptedBuffer, 62);
            }

            if (currentKey.HasValue)
                return new LoginEncryptionDetectionResult(decryptedBuffer, loginCrypt, currentKey.Value);
            else
                return new LoginEncryptionDetectionResult(decryptedBuffer);
        }

        private LoginEncryptionKey CalculateKey(Version version)
            => CalculateKey((uint)version.Major, (uint)version.Minor, (uint)version.Build);

        private LoginEncryptionKey CalculateKey(uint a, uint b, uint c)
        {
            uint temp = ((a << 9 | b) << 10 | c) ^ ((c * c) << 5);
            var key2 = (temp << 4) ^ (b * b) ^ (b * 0x0B000000) ^ (c * 0x380000) ^ 0x2C13A5FD;
            temp = (((a << 9 | c) << 10 | b) * 8) ^ (c * c * 0x0c00);
            var key3 = temp ^ (b * b) ^ (b * 0x6800000) ^ (c * 0x1c0000) ^ 0x0A31D527F;

            var key1 = key2 - 1;

            return new LoginEncryptionKey(key1, key2, key3);
        }

        private bool IsGameServerPacket(byte[] encryptedBuffer)
        {
            if (encryptedBuffer[0] != 0x80 || encryptedBuffer[30] != 0x00 || encryptedBuffer[60] != 0x00)
                return false;

            for (int toCheck = 21; toCheck <= 30; toCheck++)
            {
                if (encryptedBuffer[toCheck] != 0x00 || encryptedBuffer[toCheck + 30] != 0x00)
                    return false;
            }

            return true;
        }

    }
}
