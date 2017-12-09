using System;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Infusion.Commands;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Events;
using Infusion.Logging;
using Infusion.Proxy;
using Infusion.Utilities;

namespace Infusion.Desktop
{
    public partial class ConsoleControl : UserControl
    {
        private readonly CommandAutocompleter completer;
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        private readonly CommandHistory history = new CommandHistory();
        private readonly FlowDocument outputDocument;

        private void HandleFileLoggingException(Exception ex)
        {
            Program.Console.Error($"Error while writing logs to disk. Please, check that Infusion can write to {Program.Configuration.LogPath}.");
            Program.Console.Important("You can change the log path by setting UO.Configuration.LogPath property or disable packet logging by setting UO.Configuration.LogToFileEnabled = false in your initial script.");
            Program.Console.Debug(ex.ToString());
        }

        public ConsoleControl()
        {
            completer = new CommandAutocompleter(() => UO.CommandHandler.CommandNames);

            InitializeComponent();

            outputDocument = new FlowDocument();
            _outputViewer.Document = outputDocument;
            outputDocument.PagePadding = new Thickness(0);
            outputDocument.Background = Brushes.Black;
            outputDocument.FontFamily = _inputBlock.FontFamily;
            outputDocument.FontSize = _inputBlock.FontSize;
            outputDocument.FontStretch = _inputBlock.FontStretch;
            outputDocument.FontStyle = _inputBlock.FontStyle;


            ScriptEngine = new CSharpScriptEngine(new ScriptOutput(Dispatcher, consoleContent));

            var infusionConsoleLogger = new InfusionConsoleLogger(consoleContent, Dispatcher, Program.Configuration);

            Program.Console = new AsyncLogger(new MultiplexLogger(infusionConsoleLogger,
                new FileLogger(Program.Configuration, new CircuitBreaker(HandleFileLoggingException))));
            var commandHandler = new CommandHandler(Program.Console);

            Program.Initialize(commandHandler);
            DataContext = consoleContent;
            consoleContent.ConsoleOutput.CollectionChanged += ConsoleOutputOnCollectionChanged;

            _inputBlock.Focus();

            if (Application.Current.MainWindow != null)
                Application.Current.MainWindow.Activated += (sender, args) => FocusInputLine();
        }

        public CSharpScriptEngine ScriptEngine { get; }

        private static ScrollViewer FindScrollViewer(FlowDocumentScrollViewer flowDocumentScrollViewer)
        {
            if (VisualTreeHelper.GetChildrenCount(flowDocumentScrollViewer) == 0)
                return null;

            // Border is the first child of first child of a ScrolldocumentViewer
            var firstChild = VisualTreeHelper.GetChild(flowDocumentScrollViewer, 0);

            var border = VisualTreeHelper.GetChild(firstChild, 0) as Decorator;

            return border?.Child as ScrollViewer;
        }

        private void ConsoleOutputOnCollectionChanged(object o,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (ConsoleLine line in notifyCollectionChangedEventArgs.NewItems)
                    {
                        var newline = new Paragraph();
                        newline.Margin = new Thickness(0);
                        newline.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
                        newline.Inlines.Add(new Run(line.Message) {Foreground = line.TextBrush});
                        outputDocument.Blocks.Add(newline);
                    }

                    var scrollViewer = FindScrollViewer(_outputViewer);
                    if (scrollViewer != null && Math.Abs(scrollViewer.ScrollableHeight - scrollViewer.VerticalOffset) <
                        0.1)
                        scrollViewer.ScrollToBottom();

                    break;
                case NotifyCollectionChangedAction.Remove:
                    outputDocument.Blocks.Remove(outputDocument.Blocks.FirstBlock);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    outputDocument.Blocks.Clear();
                    break;
                default:
                    throw new NotSupportedException(
                        $"Action {notifyCollectionChangedEventArgs.Action} is not supported, Add and Remove actions are supported.");
            }
        }

        public void Initialize()
        {
            _inputBlock.Focus();
        }

        private void _inputBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            HandleInputBlockKey(e.Key);
        }

        private void RunCommand()
        {
            var text = _inputBlock.Text;
            history.EnterCommand(text);

            OnCommandEntered(text);

            _inputBlock.Text = string.Empty;
            _inputBlock.Focus();
        }

        private void OnCommandEntered(string command)
        {
            if (UO.CommandHandler.IsInvocationSyntax(command))
            {
                if (command != ",cls")
                    Program.Console.Debug(command);
                Task.Run(() =>
                {
                    UO.CommandHandler.InvokeSyntax(command);
                });
            }
            else
                UO.Say(command);
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
                HandleInputBlockKey(e.Key);
        }

        private void HandleInputBlockKey(Key key)
        {
            if (!_inputBlock.IsFocused)
            {
                _inputBlock.Focus();
                switch (key)
                {
                    case Key.Home:
                        _inputBlock.CaretIndex = 0;
                        break;
                    case Key.End:
                        _inputBlock.CaretIndex = _inputBlock.Text.Length;
                        break;
                    case Key.V:
                        if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                            _inputBlock.Paste();
                        break;
                }
            }

            switch (key)
            {
                case Key.Enter:
                    RunCommand();
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
            switch (e.Key)
            {
                case Key.PageUp:
                case Key.PageDown:
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return true;
                case Key.End:
                case Key.Home:
                    return Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
                default:
                    return false;
            }
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

        private void ConsoleControl_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightCtrl || e.Key == Key.LeftCtrl)
                return;

            if (!_inputBlock.IsFocused && !IsKeyAcceptableByConsoleOutput(e))
            {
                switch (e.Key)
                {
                    case Key.Right:
                    case Key.Left:
                    case Key.Home:
                    case Key.End:
                        HandleInputBlockKey(e.SystemKey);
                        break;
                }
            }
            else if (!_outputViewer.IsFocused && IsKeyAcceptableByConsoleOutput(e))
            {
                HandleOutputViewerKey(e.SystemKey);
            }
        }

        private void HandleOutputViewerKey(Key key)
        {
            if (!_outputViewer.IsFocused)
            {
                _outputViewer.Focus();
            }
        }
    }
}