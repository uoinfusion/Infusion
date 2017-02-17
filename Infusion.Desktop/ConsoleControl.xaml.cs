using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Infusion.Desktop.Launcher;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

namespace Infusion.Desktop
{
    public partial class ConsoleControl : UserControl
    {
        private CSharpScriptEngine scriptEngine;
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        public ConsoleControl()
        {
            InitializeComponent();

            _consoleModeComboBox.SelectedItem = _sayConsoleMode;
            scriptEngine = new CSharpScriptEngine(new ScriptOutput(Dispatcher, consoleContent));
            Program.Console = new MultiplexLogger(Program.Console,
                new InfusionConsoleLogger(consoleContent, Dispatcher), new FileLogger());
            DataContext = consoleContent;
        }

        public void Initialize(LauncherOptions options)
        {
            Task.Run(() =>
            {
                scriptEngine.AddDefaultImports().Wait();
                var scriptFileName = options.InitialScriptFileName?.Trim();
                if (!string.IsNullOrEmpty(scriptFileName))
                {
                    scriptEngine.ExecuteScript(scriptFileName).Wait();
                }
            });
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);

            if (!_inputBlock.IsFocused)
                _inputBlock.Focus();
        }

        private void _inputBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunCommand();
                _inputBlock.Focus();
                _scroller.ScrollToBottom();
            }
        }

        private void RunCommand()
        {
            string text = _inputBlock.Text;

            OnCommandEntered(text);

            _inputBlock.Text = string.Empty;
            _inputBlock.Focus();
            _scroller.ScrollToBottom();
        }

        protected virtual void OnCommandEntered(string command)
        {
            if (ReferenceEquals(_consoleModeComboBox.SelectedItem, _sayConsoleMode))
            {
                if (Injection.CommandHandler.IsInvocationSyntax(command))
                    Injection.CommandHandler.Invoke(command);
                else
                    Injection.Say(command);
            }
            else
            {
                Task.Run(() =>
                {
                    scriptEngine.Execute(command).Wait();
                });
            }
        }

        private void OnConsoleModeSelected(object sender, RoutedEventArgs e)
        {
            _inputBlock.Focus();
        }

        private void Console_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2 && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                _consoleModeComboBox.Focus();
                _consoleModeComboBox.IsDropDownOpen = true;
            }
        }
    }
}
