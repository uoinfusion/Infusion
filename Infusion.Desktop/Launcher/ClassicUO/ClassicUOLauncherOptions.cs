using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infusion.IO.Encryption.Login;

namespace Infusion.Desktop.Launcher.ClassicUO
{
    public class ClassicUOLauncherOptions
    {
        public string ClientExePath { get; set; }
        public EncryptionSetup EncryptionSetup { get; set; }
        public Version EncryptionVersion { get; set; }

        public LoginEncryptionKey? GetEncryptionKey()
        {
            if (IsEncrypted && EncryptionVersion != null)
                return LoginEncryptionKey.FromVersion(EncryptionVersion);
            else
                return null;
        }

        private bool IsEncrypted
        {
            get
            {
                switch (EncryptionSetup)
                {
                    case EncryptionSetup.EncryptedServer:
                        return true;
                    case EncryptionSetup.Autodetect:
                        return false;
                    default:
                        throw new NotImplementedException(EncryptionSetup.ToString());
                }
            }
        }

        internal bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ClientExePath))
            {
                validationMessage = "Path to ClassicUO client exe not set.";

                return false;
            }

            if (IsEncrypted && EncryptionVersion == null)
            {
                validationMessage = "Please, set encryption version.";
                return false;
            }

            validationMessage = string.Empty;
            return true;
        }
    }
}
