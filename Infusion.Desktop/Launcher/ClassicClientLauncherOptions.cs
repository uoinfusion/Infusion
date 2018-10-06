using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Infusion.Desktop.Launcher
{
    public sealed class ClassicClientLauncherOptions : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private string clientExePath;
        public string ClientExePath
        {
            get
            {
                if (string.IsNullOrEmpty(clientExePath))
                {
                    if (!string.IsNullOrEmpty(Files.RootDir))
                        clientExePath = Path.Combine(Files.RootDir, "NoCryptClient.exe");
                    else
                        clientExePath = "NoCryptClient.exe";
                }

                return clientExePath;
            }

            set
            {
                clientExePath = value;
                OnPropertyChanged();
            }
        }
    }
}
