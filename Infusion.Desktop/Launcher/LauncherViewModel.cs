using Infusion.Desktop.Launcher.ClassicUO;
using Infusion.Desktop.Profiles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Launcher
{
    internal sealed class LauncherViewModel : INotifyPropertyChanged
    {
        private LaunchProfile selectedProfile;
        private ObservableCollection<LaunchProfile> profiles = new ObservableCollection<LaunchProfile>
        {
            new LaunchProfile {Name = "new profile"}
        };
        private readonly Action<string> passwordSetter;
        private readonly Dictionary<string, ClassicUOViewModel> classicUOViewModels 
            = new Dictionary<string, ClassicUOViewModel>();

        public ClassicUOViewModel SelectedClassicUOViewModel
        {
            get
            {
                if (classicUOViewModels.TryGetValue(SelectedProfile.Id, out var model))
                {
                    return model;
                }
                else
                {
                    var newModel = new ClassicUOViewModel(SelectedProfile.LauncherOptions.ClassicUO);
                    classicUOViewModels.Add(SelectedProfile.Id, newModel);
                    return newModel;
                }
            }
        }

        private bool showPassword;
        public bool ShowPassword
        {
            get => showPassword;
            set
            {
                showPassword = value;
                OnPropertyChanged();
                OnPropertyChanged("HidePassword");
            }
        }
        public bool HidePassword => !showPassword;

        public ProtocolVersion[] ProtocolVersions { get; } = new[]
        {
            new ProtocolVersion() { Version = new Version(3, 0, 0), Label = ">= 3.0.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 0, 0), Label = ">= 7.0.0.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 9, 0), Label = ">= 7.0.9.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 16, 0), Label = ">= 7.0.16.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 18, 0), Label = ">= 7.0.18.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 33, 0), Label = ">= 7.0.33.0" },
        };

        public EncryptionVersion[] EncryptionVersions => EncryptionVersion.Versions;

        public ObservableCollection<LaunchProfile> Profiles
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

        public LauncherViewModel(Action<string> passwordSetter)
        {
            this.passwordSetter = passwordSetter;
            SelectedProfile = Profiles.First();
        }

        public LaunchProfile SelectedProfile
        {
            get => selectedProfile;
            set
            {
                selectedProfile = value;
                OnPropertyChanged();
                OnSelectedClientTypeChanged();
                passwordSetter(selectedProfile.LauncherOptions.Password);
                OnPropertyChanged("SelectedClassicUOViewModel");
            }
        }

        public string SelectedProfileName
        {
            get => SelectedProfile.Name;
            set => SelectedProfile.Name = value;
        }

        public EncryptionSetup ClassicEncryption
        {
            get => SelectedProfile.LauncherOptions.Classic.Encryption;
            set
            {
                SelectedProfile.LauncherOptions.Classic.Encryption = value;
                OnPropertyChanged();
                OnPropertyChanged("ClassicClientEncryptionVersion");
                OnPropertyChanged("EncryptionVersionRequired");
            }
        }

        public EncryptionVersion ClassicClientEncryptionVersion
        {
            get => SelectedProfile.LauncherOptions.Classic.EncryptionVersion;
            set
            {
                SelectedProfile.LauncherOptions.Classic.EncryptionVersion = value;
                OnPropertyChanged();
                OnPropertyChanged("EncryptionVersionRequired");
            }
        }

        public bool EncryptionVersionRequired
        {
            get
            {
                if (SelectedClientType != UltimaClientType.Classic)
                    return false;

                return SelectedProfile.LauncherOptions.Classic.Encryption == EncryptionSetup.EncryptedServer;
            }
        }

        public void NewProfile()
        {
            var profile = new LaunchProfile { Name = "new profile" };
            Profiles.Add(profile);
            SelectedProfile = profile;

            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged("CanDeleteSelectedProfile");
        }

        public bool CanDeleteSelectedProfile => Profiles.Count > 1;

        public bool ClassicClientOptionsVisible => SelectedProfile.LauncherOptions.ClientType == UltimaClientType.Classic;
        public bool OrionOptionsVisible => SelectedProfile.LauncherOptions.ClientType == UltimaClientType.Orion;
        public bool CrossOptionsVisible => SelectedProfile.LauncherOptions.ClientType == UltimaClientType.CrossUO;
        public bool ClassicUOOptionsVisible => SelectedProfile.LauncherOptions.ClientType == UltimaClientType.ClassicUO;

        public ProtocolVersion SelectedProtocolVersion
        {
            get => ProtocolVersions.FirstOrDefault(x => x.Version == SelectedProfile.LauncherOptions.ProtocolVersion)
                ?? ProtocolVersions.First();
            set
            {
                SelectedProfile.LauncherOptions.ProtocolVersion = value.Version;
                OnPropertyChanged();
            }
        }

        public UltimaClientType SelectedClientType
        {
            get => SelectedProfile.LauncherOptions.ClientType;
            set
            {
                SelectedProfile.LauncherOptions.ClientType = value;
                OnSelectedClientTypeChanged();
            }
        }

        private void OnSelectedClientTypeChanged()
        {
            OnPropertyChanged("SelectedClientType");
            OnPropertyChanged("ClassicClientOptionsVisible");
            OnPropertyChanged("OrionOptionsVisible");
            OnPropertyChanged("CrossOptionsVisible");
            OnPropertyChanged("ClassicUOOptionsVisible");
            OnPropertyChanged("SelectedProtocolVersion");
            OnPropertyChanged("ClassicEncryption");
            OnPropertyChanged("ClassicClientEncryptionVersion");
            OnPropertyChanged("EncryptionVersionRequired");
        }

        public void DeleteSelectedProfile()
        {
            if (Profiles.Count > 1)
            {
                var profileToRemove = SelectedProfile;
                SelectedProfile = Profiles.First(x => x != profileToRemove);
                Profiles.Remove(profileToRemove);
                ProfileRepository.DeleteProfile(profileToRemove);

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