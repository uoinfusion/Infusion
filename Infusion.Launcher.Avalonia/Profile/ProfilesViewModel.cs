using Avalonia.Diagnostics.ViewModels;
using Infusion.Desktop.Profiles;
using Infusion.LegacyApi.Console;
using Infusion.Proxy.Launcher;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Launcher.Avalonia.Profile
{
    public sealed class ProfilesViewModel : ViewModelBase
    {
        private ProfileViewModel selectedProfile;
        private ObservableCollection<ProfileViewModel> profiles;
        private readonly ILauncher launcher;

        public ObservableCollection<ProfileViewModel> Profiles
        {
            get => profiles;
            private set => RaiseAndSetIfChanged(ref profiles, value);
        }

        public ProfileViewModel SelectedProfile
        {
            get => selectedProfile;
            set => RaiseAndSetIfChanged(ref selectedProfile, value);
        }

        public ReactiveCommand<Unit, Unit> LaunchCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DeleteProfileCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> NewProfileCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> DuplicateProfileCommand { get; private set; }

        public ProfilesViewModel(ILauncher launcher)
        {
            LaunchCommand = ReactiveCommand.Create(Launch);
            NewProfileCommand = ReactiveCommand.Create(NewProfile);
            DuplicateProfileCommand = ReactiveCommand.Create(DuplicateProfile);
            DeleteProfileCommand = ReactiveCommand.Create(DeleteProfile);
            this.launcher = launcher;
        }

        public void Launch() => launcher.Launch(SelectedProfile.Profile);

        public void NewProfile()
        {
            var newProfile = new LaunchProfile
            {
                Name = "<new profile>"
            };

            var viewModel = CreateViewModel(newProfile);

            Profiles.Add(viewModel);
            SelectedProfile = viewModel;
        }

        public void DuplicateProfile()
        {
            if (SelectedProfile != null)
            {
                var duplicatedProfile = SelectedProfile.Profile.Clone();
                var viewModel = CreateViewModel(duplicatedProfile);

                duplicatedProfile.Name = "<new profile>";

                Profiles.Add(viewModel);
                SelectedProfile = viewModel;
            }
        }

        public void DeleteProfile()
        {
            if (SelectedProfile != null)
            {
                ProfileRepository.DeleteProfile(SelectedProfile.Profile);
                Profiles.Remove(SelectedProfile);
                if (Profiles.Count > 0)
                    SelectedProfile = Profiles.First();
            }
        }

        public void Load()
        {
            Profiles = new ObservableCollection<ProfileViewModel>(
                ProfileRepository.LoadProfiles().Select(x => CreateViewModel(x)));

            var selectedProfileId = ProfileRepository.LoadSelectedProfileId();
            SelectedProfile = Profiles.FirstOrDefault(x => x.Id.Equals(selectedProfileId, StringComparison.Ordinal)) ?? Profiles.FirstOrDefault();
        }

        private ProfileViewModel CreateViewModel(LaunchProfile profile)
        {
            var optionsRepository = new ProfileConfigRepository(profile, new NullConsole());
            var launcherOptions = optionsRepository.Get("launcher.avalonia", () => new LauncherOptions());

            return new ProfileViewModel(profile, launcherOptions);
        }
    }
}
