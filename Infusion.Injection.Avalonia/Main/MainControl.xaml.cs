using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Infusion.Injection.Avalonia.Main
{
    public class MainControl : UserControl
    {
        private MainViewModel viewModel;
        public MainViewModel ViewModel
        {
            get => viewModel; set
            {
                viewModel = value;
                DataContext = value;
            }
        }

        public MainControl()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
