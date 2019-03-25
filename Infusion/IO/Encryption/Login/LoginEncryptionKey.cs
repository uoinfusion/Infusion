using System;

namespace Infusion.IO.Encryption.Login
{
    public struct LoginEncryptionKey : IEquatable<LoginEncryptionKey>
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

        public override bool Equals(object obj)
        {
            if (obj is LoginEncryptionKey key)
                return key.Key1 == Key1 && key.Key2 == Key2 && key.Key3 == Key3;

            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = -2131266610;
            hashCode = hashCode * -1521134295 + Key1.GetHashCode();
            hashCode = hashCode * -1521134295 + Key2.GetHashCode();
            hashCode = hashCode * -1521134295 + Key3.GetHashCode();
            return hashCode;
        }

        public bool Equals(LoginEncryptionKey other)
        {
            return other.Key1 == Key1 && other.Key2 == Key2 && other.Key3 == Key3;
        }

        public override string ToString()
        {
            return $"0x{Key1:X8}, 0x{Key2:X8}, 0x{Key3:X8}";
        }
    }
}
