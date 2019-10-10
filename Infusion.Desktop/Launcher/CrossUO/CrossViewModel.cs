using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.CrossUO
{
    internal sealed class CrossViewModel : INotifyPropertyChanged
    {
        private CrossUOLauncherOptions options;

        public CrossViewModel(CrossUOLauncherOptions options)
        {
            this.options = options;
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

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
