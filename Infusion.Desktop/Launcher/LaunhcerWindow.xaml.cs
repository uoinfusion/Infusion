using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Forms;
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
        private readonly LauncherViewModel launcherViewModel;


        private void ShowError(string errorMessage)
        {
            Program.Console.Error(errorMessage);
        }

        internal LauncherWindow(Action<Profile> launchCallback)
        {
            this.launchCallback = launchCallback;
            InitializeComponent();
            launcherViewModel = new LauncherViewModel(pwd => passwordBox.Password = pwd);

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
            (sender as System.Windows.Controls.Button).Focus();

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

        private void OnSelectOrionPath(object sender, RoutedEventArgs e)
        {
            launcherViewModel.SelectedProfile.LauncherOptions.Orion.ClientExePath
                = SelectPath(launcherViewModel.SelectedProfile.LauncherOptions.Orion.ClientExePath, "Orion|OrionUO.exe|*.exe|*.exe");
        }

        private void OnSelectCrossPath(object sender, RoutedEventArgs e)
        {
            launcherViewModel.SelectedProfile.LauncherOptions.Cross.ClientExePath
                = SelectPath(launcherViewModel.SelectedProfile.LauncherOptions.Orion.ClientExePath, "CrossUO|crossuo.exe|*.exe|*.exe");
        }

        private string SelectPath(string initialPath, string filter)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = filter;
            openFileDialog.FileName = initialPath;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog.FileName;
            }

            return initialPath;
        }

        private void OnSelectClassicPath(object sender, RoutedEventArgs e)
        {
            launcherViewModel.SelectedProfile.LauncherOptions.Classic.ClientExePath
                = SelectPath(launcherViewModel.SelectedProfile.LauncherOptions.Classic.ClientExePath, "*.exe|*.exe");

        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            launcherViewModel.SelectedProfile.LauncherOptions.Password = passwordBox.Password;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            launcherViewModel.ShowPassword = true;
            launcherViewModel.SelectedProfile.LauncherOptions.Password = passwordBox.Password;
            passwordTextBox.Text = passwordBox.Password;
            passwordBox.Visibility = Visibility.Collapsed;
            passwordTextBox.Visibility = Visibility.Visible;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            launcherViewModel.ShowPassword = false;
            passwordTextBox.Text = string.Empty;
            passwordBox.Visibility = Visibility.Visible;
            passwordTextBox.Visibility = Visibility.Collapsed;
        }
    }
}