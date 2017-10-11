using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Infusion.Desktop.Profiles;
using Infusion.Proxy;
using Newtonsoft.Json;

namespace Infusion.Desktop.Launcher
{
    internal partial class LauncherWindow : Window
    {
        private readonly Action<Profile> launchCallback;
        private readonly LauncherViewModel launcherViewModel = new LauncherViewModel();


        private void ShowError(string errorMessage)
        {
            Program.Console.Error(errorMessage);
        }

        internal LauncherWindow(Action<Profile> launchCallback)
        {
            this.launchCallback = launchCallback;
            InitializeComponent();

            var profiles = ProfileRepositiory.LoadProfiles();
            if (profiles != null)
                launcherViewModel.Profiles = new ObservableCollection<Profile>(profiles);

            string selectedProfileId = ProfileRepositiory.LoadSelectedProfileId();
            if (profiles != null && !string.IsNullOrEmpty(selectedProfileId))
            {
                launcherViewModel.SelectedProfile = launcherViewModel.Profiles.FirstOrDefault(p => p.Id == selectedProfileId) ?? launcherViewModel.Profiles.FirstOrDefault();
            }
            DataContext = launcherViewModel;
        }

        private async void OnLaunchButtonClicked(object sender, RoutedEventArgs e)
        {
            ProfileRepositiory.SaveProfiles(launcherViewModel.Profiles);
            ProfileRepositiory.SelectedProfile = launcherViewModel.SelectedProfile;
            ProfileRepositiory.SaveSelectedProfileId(launcherViewModel.SelectedProfile.Id);

            var launcherOptions = launcherViewModel.SelectedProfile.LauncherOptions;
            string validationMessage;
            if (!launcherOptions.Validate(out validationMessage))
            {
                ShowError(validationMessage);
                return;
            }

            IsEnabled = false;
            string originalTitle = Title;

            string message = $"Connecting to {launcherOptions.ServerEndpoint}";;
            Title = message;
            Program.Console.Info(message);

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

            launchCallback(launcherViewModel.SelectedProfile);

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
                    aggregateException.InnerExceptions.Select(x => x.ToString())
                        .Aggregate((l, r) => l + Environment.NewLine + r);
            }
            else
            {
                message = exception.ToString();
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
    }
}