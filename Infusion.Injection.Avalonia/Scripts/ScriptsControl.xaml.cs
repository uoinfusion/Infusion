using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace Infusion.Injection.Avalonia.Scripts
{
    public class ScriptsControl : UserControl
    {
        private readonly ScriptsViewModel viewModel;

        public ScriptsControl()
        {
            this.InitializeComponent();

            viewModel = new ScriptsViewModel(OpenFile);
            DataContext = viewModel;
        }

        private Task<string[]> OpenFile()
        {
            var dialog = new OpenFileDialog();
            dialog.AllowMultiple = false;

            return dialog.ShowAsync(this.VisualRoot as Window);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public void SetServices(IScriptServices scriptServices) 
            => viewModel.SetServices(scriptServices);
    }
}
