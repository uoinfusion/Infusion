using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Infusion.Launcher.Avalonia.Profile;

namespace Infusion.Launcher.Avalonia
{
    public class MainWindow : Window
    {
        private readonly ProfilesViewModel viewModel;

        public MainWindow(ILauncher launcher)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            viewModel = new ProfilesViewModel(launcher);
            viewModel.Launched += () => Close();
            DataContext = viewModel;
            viewModel.Load();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
