using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Infusion.Desktop.Profiles;

namespace Infusion.Desktop.Launcher
{
    internal sealed class LauncherViewModel : INotifyPropertyChanged
    {
        private Profile selectedProfile;
        private ObservableCollection<Profile> profiles = new ObservableCollection<Profile>
        {
            new Profile {Name = "new profile"}
        };

        public ObservableCollection<Profile> Profiles
        {
            get => profiles;
            set
            {
                profiles = value;
                if (profiles.Any())
                {
                    SelectedProfile = profiles.First();
                }
            }
        }

        public LauncherViewModel()
        {
            SelectedProfile = Profiles.First();
        }

        public Profile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value; 
                OnPropertyChanged();
            }
        }

        public string SelectedProfileName
        {
            get => SelectedProfile.Name;
            set => SelectedProfile.Name = value;
        }

        public void NewProfile()
        {
            var profile = new Profile {Name = "new profile"};
            Profiles.Add(profile);
            SelectedProfile = profile;
            
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("CanDeleteSelectedProfile");
        }

        public bool CanDeleteSelectedProfile => Profiles.Count > 1;

        public void DeleteSelectedProfile()
        {
            if (Profiles.Count > 1)
            {
                var profileToRemove = SelectedProfile;
                SelectedProfile = Profiles.First(x => x != profileToRemove);
                Profiles.Remove(profileToRemove);
                ProfileRepositiory.DeleteProfile(profileToRemove);

            }
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("CanDeleteSelectedProfile");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}