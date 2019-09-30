using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.ClassicUO
{
    internal sealed class ClassicUOViewModel : INotifyPropertyChanged
    {
        private readonly ClassicUOLauncherOptions options;

        private ClassicUOEncryptionType ToClassicUOEncryption(EncryptionSetup type)
        {
            switch (type)
            {
                case EncryptionSetup.Autodetect:
                    return ClassicUOEncryptionType.NoEcnryption;
                case EncryptionSetup.EncryptedClient:
                    return ClassicUOEncryptionType.NoEcnryption;
                case EncryptionSetup.EncryptedServer:
                    return ClassicUOEncryptionType.AddEncryption;
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        private EncryptionSetup ToEncryptionSetup(ClassicUOEncryptionType type)
        {
            switch (type)
            {
                case ClassicUOEncryptionType.AddEncryption:
                    return EncryptionSetup.EncryptedServer;
                case ClassicUOEncryptionType.NoEcnryption:
                    return EncryptionSetup.Autodetect;
                default:
                    throw new NotImplementedException(type.ToString());
            }
        }

        public ClassicUOEncryptionType EncryptionType
        {
            get => ToClassicUOEncryption(options.EncryptionSetup);
            set
            {
                options.EncryptionSetup = ToEncryptionSetup(value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(EncryptionVersionRequired));
            }
        }

        public bool EncryptionVersionRequired => EncryptionType == ClassicUOEncryptionType.AddEncryption;

        public Version EncryptionVersion
        {
            get => options.EncryptionVersion;
            set
            {
                options.EncryptionVersion = value;
                OnPropertyChanged();
            }
        }

        public string ClientExePath
        {
            get => options.ClientExePath;
            set
            {
                options.ClientExePath = value;
                OnPropertyChanged();
            }
        }

        public ClassicUOViewModel(ClassicUOLauncherOptions options)
        {
            this.options = options;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
