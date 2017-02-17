using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Newtonsoft.Json;

namespace Infusion.Desktop.Launcher
{
    public partial class MainWindow : Window
    {
        private readonly LauncherViewModel launcherViewModel = new LauncherViewModel();
        private readonly InfusionSettings settings = (InfusionSettings)SettingsBase.Synchronized(InfusionSettings.Default);
        private readonly DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();


        private void ShowError(string errorMessage)
        {
            _errorTextBlock.Visibility = Visibility.Visible;
            _errorTextBlock.Text = errorMessage;
            
            dispatcherTimer.Interval = TimeSpan.FromSeconds(5);
            dispatcherTimer.Start();
        }

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

            dispatcherTimer.Tick += (sender, args) => HideError();
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

            var launcherOptions = launcherViewModel.SelectedProfile.LauncherOptions;
            string validationMessage;
            if (!launcherOptions.Validate(out validationMessage))
            {
                ShowError(validationMessage);
                return;
            }

            IsEnabled = false;
            string originalTitle = Title;

            Title = $"Connecting to {launcherOptions.ServerEndpoint}";

            try
            {
                await Launcher.Launch(launcherOptions);
            }
            catch (AggregateException ex)
            {
                HandleException(ex, originalTitle);
                return;
            }
            catch (Exception ex)
            {
                HandleException(ex, originalTitle);
                return;
            }

            var infusionWindow = new InfusionWindow();
            Application.Current.MainWindow = infusionWindow;
            infusionWindow.Title = $"{launcherViewModel.SelectedProfile.Name}";
            infusionWindow.Show();
            infusionWindow.Initialize(launcherViewModel.SelectedProfile.LauncherOptions);

            Close();
        }

        private void HandleException(Exception exception, string title)
        {
            IsEnabled = true;
            Title = title;

            string message;
            var aggregateException = exception as AggregateException;
            if (aggregateException != null)
            {
                message =
                    aggregateException.InnerExceptions.Select(x => x.Message)
                        .Aggregate((l, r) => l + Environment.NewLine + r);
            }
            else
            {
                message = exception.Message;
            }

            ShowError(message);
        }

        private void OnNewProfileButtonClick(object sender, RoutedEventArgs e)
        {
            launcherViewModel.NewProfile();
        }

        private void OnDeleteProfileButtonClick(object sender, RoutedEventArgs e)
        {
            launcherViewModel.DeleteSelectedProfile();
        }

        private void _errorTextBlock_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HideError();
        }

        private void HideError()
        {
            _errorTextBlock.Text = string.Empty;
            _errorTextBlock.Visibility = Visibility.Collapsed;
        }
    }
}