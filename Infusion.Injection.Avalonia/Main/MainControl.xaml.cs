using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Injection.Avalonia.Main
{
    public class MainControl : UserControl
    {
        public MainViewModel ViewModel { get; }

        public MainControl()
        {
            this.InitializeComponent();

            ViewModel = new MainViewModel();
            DataContext = ViewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
