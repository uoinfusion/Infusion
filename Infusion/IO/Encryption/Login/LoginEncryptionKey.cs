namespace Infusion.IO.Encryption.Login
{
    internal struct LoginEncryptionKey
    {
        public static readonly LoginEncryptionKey[] Keys =
        {
            new LoginEncryptionKey(0x2cc3ed9d, 0xa374227f)
        };

        public uint Key1 { get; }
        public uint Key2 { get; }

        public LoginEncryptionKey(uint key1, uint key2)
        {
            Key1 = key1;
            Key2 = key2;
        }
    }
}
