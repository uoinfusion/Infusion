using Infusion.IO.Encryption.Login;
using Infusion.Proxy.Launcher.Classic;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Launcher.Avalonia.Profile.Classic
{
    public sealed class ClassicOptionsViewModel : ReactiveObject
    {
        private readonly ClassicClientLauncherOptions classic;

        public ClassicOptionsViewModel(ClassicClientLauncherOptions classic)
        {
            this.classic = classic;
        }

        public EncryptionSetup SelectedEncryptionSetup
        {
            get => classic.Encryption;
            set
            {
                classic.Encryption = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(EncryptionVersionRequired));
            }
        }

        public bool EncryptionVersionRequired
            => SelectedEncryptionSetup == EncryptionSetup.EncryptedServer;

        public string EncryptionVersion
        {
            get => classic.EncryptionVersion.Name;
            set
            {
                var version = Version.Parse(value);
                classic.EncryptionVersion = new EncryptionVersion(value, LoginEncryptionKey.Calculate(version));
            }
        }

        public string ClientPath
        {
            get => classic.ClientExePath;
            set
            {
                classic.ClientExePath = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
