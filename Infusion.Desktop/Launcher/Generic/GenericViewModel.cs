using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Launcher.Generic
{
    internal sealed class GenericViewModel : INotifyPropertyChanged
    {
        private GenericLauncherOptions options;

        public GenericViewModel(GenericLauncherOptions options)
        {
            this.options = options;
        }

        public string DataPath
        {
            get => options.DataPath;
            set
            {
                options.DataPath = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get => options.Port.ToString();
            set
            {
                if (ushort.TryParse(value, out var port))
                    options.Port = port;
                else
                    options.Port = 0;
                OnPropertyChanged();
            }
        }

        public EncryptionSetup Encryption
        {
            get => options.Encryption;
            set
            {
                options.Encryption = value;
                OnPropertyChanged();
                OnPropertyChanged("EncryptionVersion");
                OnPropertyChanged("EncryptionVersionRequired");
            }
        }

        public Version EncryptionVersion
        {
            get => options.EncryptionVersion;
            set
            {
                options.EncryptionVersion = value;
                OnPropertyChanged();
                OnPropertyChanged("EncryptionVersionRequired");
            }
        }

        public bool EncryptionVersionRequired 
            => options.Encryption == EncryptionSetup.EncryptedServer;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
