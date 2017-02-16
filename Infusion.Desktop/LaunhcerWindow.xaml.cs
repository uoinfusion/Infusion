using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;

namespace Infusion.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly LauncherViewModel launcherViewModel = new LauncherViewModel();
        private readonly InfusionSettings settings = (InfusionSettings)SettingsBase.Synchronized(InfusionSettings.Default);

        public MainWindow()
        {
            InitializeComponent();

            var profiles = LoadProfiles();
            if (profiles != null)
                launcherViewModel.Profiles = new ObservableCollection<Profile>(profiles);

            if (profiles != null && !string.IsNullOrEmpty(settings.SelectedProfileId))
            {
                launcherViewModel.SelectedProfile = launcherViewModel.Profiles.FirstOrDefault(p => p.Id == settings.SelectedProfileId);
            }
            DataContext = launcherViewModel;
        }

        private IEnumerable<Profile> LoadProfiles()
        {
            var profiles = settings.Profiles;

            if (string.IsNullOrEmpty(profiles))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<Profile[]>(profiles);
            }
            catch
            {
                // just throw away potentially malformed settings
                return null;
            }
        }


        private async void OnLaunchButtonClicked(object sender, RoutedEventArgs e)
        {
            InfusionSettings.Default.Profiles = JsonConvert.SerializeObject(launcherViewModel.Profiles);
            InfusionSettings.Default.SelectedProfileId = launcherViewModel.SelectedProfile.Id;
            InfusionSettings.Default.Save();
            IsEnabled = false;
            string originalTitle = Title;

            var launcherOptions = launcherViewModel.SelectedProfile.LauncherOptions;
            Title = $"Connecting to {launcherOptions.ServerEndpoint}";

            try
            {
                await Launcher.Launch(launcherOptions);
            }
            catch (Exception)
            {
                IsEnabled = true;
                Title = originalTitle;
                MessageBox.Show(this, $"Cannot connect to {launcherOptions.ServerEndpoint}");
                return;
            }
            
            var infusionWindow = new InfusionWindow();
            Application.Current.MainWindow = infusionWindow;
            infusionWindow.Title = $"{launcherViewModel.SelectedProfile.Name}";
            infusionWindow.Show();
            infusionWindow.Initialize(launcherViewModel.SelectedProfile.LauncherOptions);

            Close();
        }

        private void OnNewProfileButtonClick(object sender, RoutedEventArgs e)
        {
            launcherViewModel.NewProfile();
        }

        private void OnDeleteProfileButtonClick(object sender, RoutedEventArgs e)
        {
            launcherViewModel.DeleteSelectedProfile();
        }
    }
}