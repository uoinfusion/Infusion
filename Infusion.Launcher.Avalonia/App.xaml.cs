using Avalonia;
using Avalonia.Markup.Xaml;

namespace Infusion.Launcher.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
