using System.Linq;
using System.Text;
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
        private static readonly Key[] acceptableKeys =
        {
            Key.LeftCtrl,
            Key.RightCtrl
        };

        private readonly CommandAutocompleter completer;
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        private readonly CommandHistory history = new CommandHistory();

        public ConsoleControl()
        {
            completer = new CommandAutocompleter(() => Legacy.CommandHandler.CommandNames);

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
            _inputBlock.Focus();
        }

        private void _inputBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyDown(e);
        }

        private void RunCommand()
        {
            var text = _inputBlock.Text;
            history.EnterCommand(text);

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
            {
                _inputBlock.Focus();
                HandleKeyDown(e);
            }
        }

        private void HandleKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    RunCommand();
                    _inputBlock.Focus();
                    _scroller.ScrollToBottom();
                    break;

                case Key.Escape:
                    _inputBlock.Text = string.Empty;
                    break;

                case Key.Tab:
                    Autocomplete();
                    break;

                case Key.Up:
                    if (Keyboard.IsKeyDown(Key.RightAlt) || Keyboard.IsKeyDown(Key.LeftAlt))
                        RestoreOlderCommand();
                    break;
            }
        }

        private void RestoreOlderCommand()
        {
            RestoreCommand(history.GetOlder());
        }

        private void RestoreNewerCommand()
        {
            RestoreCommand(history.GetNewer());
        }

        private void RestoreCommand(string command)
        {
            if (!string.IsNullOrEmpty(command))
            {
                _inputBlock.Text = command;
                _inputBlock.CaretIndex = command.Length;
            }
        }

        private void Autocomplete()
        {
            var autocompletion = completer.Autocomplete(_inputBlock.Text);

            if (autocompletion.IsAutocompleted)
            {
                _inputBlock.Text = autocompletion.AutocompletedCommandLine;
                _inputBlock.CaretIndex = _inputBlock.Text.Length;
            }

            if (autocompletion.PotentialCommandNames.Length > 1)
            {
                var result = new StringBuilder();
                result.AppendLine("Available commands:");

                foreach (var name in autocompletion.PotentialCommandNames)
                    result.AppendLine(name);

                Program.Console.Info(result.ToString());
            }
        }

        private static bool IsKeyAcceptableByConsoleOutput(KeyEventArgs e)
        {
            return acceptableKeys.Any(k => Keyboard.IsKeyDown(k));
        }

        private void _inputBlock_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.SystemKey)
            {
                case Key.Up:
                    RestoreOlderCommand();
                    break;
                case Key.Down:
                    RestoreNewerCommand();
                    break;
            }
        }
    }
}