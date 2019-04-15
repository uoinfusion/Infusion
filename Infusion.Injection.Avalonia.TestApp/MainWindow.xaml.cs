using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Threading.Tasks;

namespace Infusion.Injection.Avalonia.TestApp
{
    public class MainWindow : Window
    {
        private TestInjectionObjectServices objectServices;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            var injectionWindowHandler = new InjectionWindowHandler();

            objectServices = new TestInjectionObjectServices();
            objectServices.Set("Initial Object", 0x0000DEAD);
            objectServices.Set("Second Initial Object", 0x0000BEEF);

            this.FindControl<Button>("AddObjectButton").Click += (sender, e) => AddObject();
            this.FindControl<Button>("RemoveObjectButton").Click += (sender, e) => RemoveObject();

            injectionWindowHandler.Open(objectServices);
        }

        public TextBox ObjectName => this.FindControl<TextBox>("ObjectName");
        public TextBox ObjectId => this.FindControl<TextBox>("ObjectId");

        public void AddObject() => Task.Run(() => objectServices.Set(ObjectName.Text, int.Parse(ObjectId.Text)));
        public void RemoveObject() => Task.Run(() => objectServices.Remove(ObjectName.Text));
    }
}
