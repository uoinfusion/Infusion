using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Launcher
{
    public class OrionLanuncherOptions : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ClientExePath))
            {
                validationMessage = "Path to Orion client exe not set.";

                return false;
            }

            validationMessage = string.Empty;
            return true;
        }
    }
}
