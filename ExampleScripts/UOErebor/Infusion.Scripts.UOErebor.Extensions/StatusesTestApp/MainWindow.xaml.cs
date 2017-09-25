using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infusion.Scripts.UOErebor.Extensions.StatusBars;

namespace StatusesTestApp
{
    public partial class MainWindow : Window
    {
        private readonly StatusesViewModel viewModel;
        private readonly Statuses statusBars;

        public MainWindow()
        {
            InitializeComponent();

            viewModel = new StatusesViewModel();
            viewModel.FirstStatusBar = new StatusBar(1) { CurrentHealth = 0, MaxHealth = 100, Name = "first", Type = StatusBarType.Friend};
            viewModel.SecondStatusBar = new StatusBar(2) { CurrentHealth = 50, MaxHealth = 100, Name = "second", Type = StatusBarType.Enemy };
            viewModel.ThirdStatusBar = new StatusBar(3) { CurrentHealth = 100, MaxHealth = 100, Name = "third", Type = StatusBarType.Pet };
            DataContext = viewModel;

            statusBars = new Statuses();
            statusBars.Open();
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

    internal class StatusesViewModel
    {
        public StatusBar FirstStatusBar { get; set; }
        public StatusBar SecondStatusBar { get; set; }
        public StatusBar ThirdStatusBar { get; set; }
    }
}
