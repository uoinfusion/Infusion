using Avalonia;
using Avalonia.Markup.Xaml;

namespace Infusion.Injection.Avalonia.TestApp
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
