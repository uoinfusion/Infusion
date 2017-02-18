using System.Windows.Controls;

namespace Infusion.Desktop.Profiles
{
    internal partial class OptionsControl : UserControl
    {
        public OptionsControl()
        {
            InitializeComponent();

            DataContext = ProfileRepositiory.SelectedProfile.Options;
        }
    }
}
