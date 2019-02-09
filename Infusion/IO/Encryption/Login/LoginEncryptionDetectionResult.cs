namespace Infusion.IO.Encryption.Login
{
    internal sealed class LoginEncryptionDetectionResult
    {
        public byte[] DecryptedPacket { get; }
        public LoginCrypt Encryption { get; }
        public LoginEncryptionKey? Key { get; set; }

        public LoginEncryptionDetectionResult(byte[] decryptedPacket, LoginCrypt encryption, LoginEncryptionKey key)
        {
            DecryptedPacket = decryptedPacket;
            Encryption = encryption;
            Key = key;
        }

        public LoginEncryptionDetectionResult(byte[] decryptedPacket)
        {
            DecryptedPacket = decryptedPacket;
            Key = null;
            Encryption = null;
        }
    }
}
