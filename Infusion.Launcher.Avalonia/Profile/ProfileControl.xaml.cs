using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Launcher.Avalonia.Profile
{
    public class ProfileControl : UserControl
    {
        public ProfileControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
