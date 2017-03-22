using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infusion.Desktop.Launcher;

namespace Infusion.Desktop.Profiles
{
    internal sealed class Profile : INotifyPropertyChanged
    {
        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value; 
                OnPropertyChanged();
            }
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public LauncherOptions LauncherOptions { get; set; } = new LauncherOptions();
        public Options Options { get; set; } = new Options();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
