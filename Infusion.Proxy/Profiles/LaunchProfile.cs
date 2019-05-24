using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Profiles
{
    public sealed class LaunchProfile : INotifyPropertyChanged, ICloneable
    {
        private string name;

        public string Name
        {
            get => name;
            set
            {
                name = value; 
                OnPropertyChanged();
            }
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Dictionary<string, object> Options { get; internal set; } = new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        object ICloneable.Clone() => Clone();

        public LaunchProfile Clone()
        {
            var newProfile = new LaunchProfile();
            newProfile.Name = this.name;

            foreach (var optionPair in Options)
            {
                newProfile.Options[optionPair.Key] = optionPair.Value;
            }

            return newProfile;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
