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
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using Infusion.Proxy.Logging;
using RoslynPad;
using RoslynPad.UI;

namespace Infusion.Desktop
{
    public partial class ConsoleControl : UserControl
    {
        public CSharpScriptEngine ScriptEngine { get; private set; }
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        public ConsoleControl()
        {
            InitializeComponent();

            ScriptEngine = new CSharpScriptEngine(new ScriptOutput(Dispatcher, consoleContent));
            Program.Console = new MultiplexLogger(Program.Console,
                new InfusionConsoleLogger(consoleContent, Dispatcher, Program.Configuration), new FileLogger(Program.Configuration));
            DataContext = consoleContent;

            _inputBlock.Focus();
        }

        public void Initialize()
        {
            Task.Run(() =>
            {
                ScriptEngine.AddDefaultImports().Wait();
            });

            _inputBlock.Focus();
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
            if (Legacy.CommandHandler.IsInvocationSyntax(command))
                Legacy.CommandHandler.Invoke(command);
            else
                Legacy.Say(command);
        }

        private void ConsoleControl_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_inputBlock.IsFocused)
                _inputBlock.Focus();
        }

        public void Clear()
        {
            consoleContent.Clear();
        }
    }
}
