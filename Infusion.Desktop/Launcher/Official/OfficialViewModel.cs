using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.Official
{
    internal sealed class OfficialViewModel : INotifyPropertyChanged
    {
        private OfficialClientLauncherOptions options;

        public OfficialViewModel(OfficialClientLauncherOptions options)
        {
            this.options = options;
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

        public string ClientExePath
        {
            get => options.ClientExePath;
            set
            {
                OnPropertyChanged();
                options.ClientExePath = value;
            }
        }

        public bool EncryptionVersionRequired
        {
            get
            {
                return options.Encryption == EncryptionSetup.EncryptedServer;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
