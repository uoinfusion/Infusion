using System;

namespace Infusion.IO.Encryption.Login
{
    public struct LoginEncryptionKey : IEquatable<LoginEncryptionKey>
    {
        public static readonly LoginEncryptionKey NullKey = new LoginEncryptionKey();

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

        public static LoginEncryptionKey? FromVersion(Version version)
        {
            if (version == null)
                return NullKey;

            return CalculateKey(version);
        }

        private static LoginEncryptionKey CalculateKey(Version version)
            => CalculateKey((uint)version.Major, (uint)version.Minor, (uint)version.Build);

        private static LoginEncryptionKey CalculateKey(uint a, uint b, uint c)
        {
            uint temp = ((a << 9 | b) << 10 | c) ^ ((c * c) << 5);
            var key2 = (temp << 4) ^ (b * b) ^ (b * 0x0B000000) ^ (c * 0x380000) ^ 0x2C13A5FD;
            temp = (((a << 9 | c) << 10 | b) * 8) ^ (c * c * 0x0c00);
            var key3 = temp ^ (b * b) ^ (b * 0x6800000) ^ (c * 0x1c0000) ^ 0x0A31D527F;

            var key1 = key2 - 1;

            return new LoginEncryptionKey(key1, key2, key3);
        }

    }
}
