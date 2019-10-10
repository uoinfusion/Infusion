using Infusion.IO.Encryption.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    public sealed class EncryptionVersion
    {
        public string Name { get; }
        public LoginEncryptionKey Key { get; }

        public EncryptionVersion(string name, LoginEncryptionKey key)
        {
            Name = name;
            Key = key;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is EncryptionVersion version)
            {
                if (string.IsNullOrEmpty(version.Name))
                    return string.IsNullOrEmpty(Name);

                return version.Name.Equals(Name, StringComparison.Ordinal);
            }

            return false;
        }

        public override int GetHashCode() => Name.GetHashCode();
    }
}
