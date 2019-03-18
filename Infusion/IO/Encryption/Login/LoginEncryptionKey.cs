namespace Infusion.IO.Encryption.Login
{
    public struct LoginEncryptionKey
    {
        public uint Key1 { get; }
        public uint Key2 { get; }
        public uint Key3 { get; }

        public LoginEncryptionKey(uint key2, uint key3)
            : this(key2 - 1, key2, key3)
        {
        }

        public LoginEncryptionKey(uint key1, uint key2, uint key3)
        {
            Key1 = key1;
            Key2 = key2;
            Key3 = key3;
        }
    }
}
