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

namespace Infusion.Desktop.Launcher.Official
{
    /// <summary>
    /// Interaction logic for OfficialControl.xaml
    /// </summary>
    public partial class OfficialControl : UserControl
    {
        public OfficialControl()
        {
            InitializeComponent();
        }

        private void OnSelectExePath(object sender, RoutedEventArgs e)
        {
            if (DataContext is LauncherViewModel parentViewModel)
            {
                var viewModel = parentViewModel.SelectedOfficialViewModel;
                viewModel.ClientExePath = PathPickerHelper.SelectPath(viewModel.ClientExePath, "*.exe|*.exe");
            }
        }
    }
}
