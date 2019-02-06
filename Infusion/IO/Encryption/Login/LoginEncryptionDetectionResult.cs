namespace Infusion.IO.Encryption.Login
{
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
}
