using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infusion.IO.Encryption.Login;

namespace Infusion.Desktop.Launcher.ClassicUO
{
    public class ClassicUOLauncherOptions : INotifyPropertyChanged
    {
        private string clientExePath;
        public string ClientExePath
        {
            get => clientExePath;
            set
            {
                clientExePath = value;
                OnPropertyChanged();
            }
        }

        private EncryptionSetup encryptionSetup;
        public EncryptionSetup EncryptionSetup
        {
            get => encryptionSetup;
            set
            {
                encryptionSetup = value;
                OnPropertyChanged();
            }
        }

        private Version encryptionVersion;
        public Version EncryptionVersion
        {
            get => encryptionVersion;
            set
            {
                encryptionVersion = value;
                OnPropertyChanged();
            }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
