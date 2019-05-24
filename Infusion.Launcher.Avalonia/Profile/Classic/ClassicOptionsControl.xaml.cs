using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Launcher.Avalonia.Profile.Classic
{
    public class ClassicOptionsControl : UserControl
    {
        public ClassicOptionsControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
