using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Launcher.Avalonia.Profile.Cross
{
    public class CrossOptionsControl : UserControl
    {
        public CrossOptionsControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
