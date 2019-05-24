using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Launcher.Avalonia.Profile.Orion
{
    public class OrionOptionsControl : UserControl
    {
        public OrionOptionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
