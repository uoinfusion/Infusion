using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

namespace StatusesTestApp
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void SwitchToThisWindow(IntPtr hWnd, bool turnOn);

        private readonly Statuses statusBars;
        private readonly StatusesViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new StatusesViewModel();
            viewModel.FirstStatusBar = new StatusBar(1)
            {
                CurrentHealth = 0,
                MaxHealth = 100,
                Name = "first",
                Type = StatusBarType.Friend
            };
            viewModel.SecondStatusBar = new StatusBar(2)
            {
                CurrentHealth = 50,
                MaxHealth = 100,
                Name = "second",
                Type = StatusBarType.Enemy
            };
            viewModel.ThirdStatusBar = new StatusBar(3)
            {
                CurrentHealth = 100,
                MaxHealth = 100,
                Name = "third",
                Type = StatusBarType.Pet
            };
            DataContext = viewModel;

            statusBars = new Statuses("Test");
            statusBars.Open();
            statusBars.MobileTargeted += (sender, id) =>
            {
                viewModel.TargetedMobileId = id.ToString();
            };
        }

        private void OnFirstStatusBarVisibleCheckBoxChecked(object sender, RoutedEventArgs e)
        {
            statusBars.Add(viewModel.FirstStatusBar);
        }

        private void _firstStatusBarVisibleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            statusBars.Remove(viewModel.FirstStatusBar);
        }

        private void _secondStatusBarVisibleCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            statusBars.Add(viewModel.SecondStatusBar);
        }

        private void _secondStatusBarVisibleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            statusBars.Remove(viewModel.SecondStatusBar);
        }

        private void _thirdStatusBarVisibleCheckBox_OnChecked(object sender, RoutedEventArgs e)
        {
            statusBars.Add(viewModel.ThirdStatusBar);
        }

        private void _thirdStatusBarVisibleCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            statusBars.Remove(viewModel.ThirdStatusBar);
        }
    }

    internal class StatusesViewModel : INotifyPropertyChanged
    {
        private string targetedMobileId;

        public string TargetedMobileId
        {
            get => targetedMobileId;
            set
            {
                targetedMobileId = value;
                OnPropertyChanged();
            }
        }

        public StatusBar FirstStatusBar { get; set; }
        public StatusBar SecondStatusBar { get; set; }
        public StatusBar ThirdStatusBar { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}