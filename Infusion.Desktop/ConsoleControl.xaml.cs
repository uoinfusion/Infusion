using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using Infusion.Proxy.Logging;

namespace Infusion.Desktop
{
    public partial class ConsoleControl : UserControl
    {
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        public ConsoleControl()
        {
            InitializeComponent();

            ScriptEngine = new CSharpScriptEngine(new ScriptOutput(Dispatcher, consoleContent));
            Program.Console = new MultiplexLogger(Program.Console,
                new InfusionConsoleLogger(consoleContent, Dispatcher, Program.Configuration),
                new FileLogger(Program.Configuration));
            DataContext = consoleContent;

            _inputBlock.Focus();

            Application.Current.MainWindow.Activated += (sender, args) => FocusInputLine();
        }

        public CSharpScriptEngine ScriptEngine { get; }

        public void Initialize()
        {
            Task.Run(() => { ScriptEngine.AddDefaultImports().Wait(); });

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
            var text = _inputBlock.Text;

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

        private void FocusInputLine()
        {
            if (!_inputBlock.IsFocused)
                _inputBlock.Focus();
        }

        public void Clear()
        {
            consoleContent.Clear();
        }

        private void ConsoleControl_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_inputBlock.IsFocused && !IsKeyAcceptableByConsoleOutput(e))
                _inputBlock.Focus();
        }

        private static bool IsKeyAcceptableByConsoleOutput(KeyEventArgs e) =>  Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
    }
}