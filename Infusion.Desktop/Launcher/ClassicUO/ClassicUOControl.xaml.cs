using Infusion.Desktop.Launcher.ClassicUO;
using System;
using System.Collections.Generic;
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

namespace Infusion.Desktop.Launcher
{
    /// <summary>
    /// Interaction logic for ClassicUOControl.xaml
    /// </summary>
    public partial class ClassicUOControl : UserControl
    {
        public ClassicUOControl()
        {
            InitializeComponent();
        }

        private void OnSelectPath(object sender, RoutedEventArgs e)
        {
            if (DataContext is LauncherViewModel parentViewModel)
            {
                var viewModel = parentViewModel.SelectedClassicUOViewModel;
                viewModel.ClientExePath = PathPickerHelper.SelectPath(viewModel.ClientExePath, "*.exe|*.exe");
            }
        }
    }
}
