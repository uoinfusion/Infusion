using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop
{
    public class LauncherViewModel : INotifyPropertyChanged
    {
        private Profile selectedProfile;
        private ObservableCollection<Profile> profiles = new ObservableCollection<Profile>
        {
            new Profile {Name = "new profile"}
        };

        public ObservableCollection<Profile> Profiles
        {
            get { return profiles; }
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
            get { return selectedProfile; }
            set
            {
                selectedProfile = value; 
                OnPropertyChanged();
            }
        }

        public string SelectedProfileName
        {
            get { return SelectedProfile.Name; }
            set
            {
                SelectedProfile.Name = value;
            }
        }

        public void NewProfile()
        {
            var profile = new Profile {Name = "new profile", Id = Guid.NewGuid().ToString()};
            Profiles.Add(profile);
            SelectedProfile = profile;
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
            }
            OnPropertyChanged("CanDeleteSelectedProfile");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}