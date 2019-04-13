using Avalonia;
using Avalonia.Markup.Xaml;

namespace Infusion.Desktop
{
    public class AvaloniaApp : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
