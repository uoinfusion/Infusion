using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class Command
    {
        private readonly Action commandAction;
        private readonly CommandExecutionMode executionMode;
        private readonly Action<string> parameterizedCommandAction;
        private static readonly ThreadLocal<int> NestingLevel = new ThreadLocal<int>();

        public Command(string name, Action commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            this.commandAction = commandAction;
            Name = name;
            this.executionMode = executionMode;
        }

        public Command(string name, Action<string> commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            Name = name;
            parameterizedCommandAction = commandAction;
            this.executionMode = executionMode;
        }

        public string Name { get; }

        internal void Terminate()
        {
            cancellationTokenSource?.Cancel();
        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        internal void Invoke()
        {
            if (commandAction != null)
                Invoke(commandAction);
            else if (parameterizedCommandAction != null)
                Invoke(() => parameterizedCommandAction(null));
        }

        internal void Invoke(string parameters)
        {
            Invoke(() => parameterizedCommandAction(parameters));
        }

        private void Invoke(Action action)
        {
            switch (executionMode)
            {
                case CommandExecutionMode.Direct:
                    action();
                    break;
                case CommandExecutionMode.Normal:
                    if (NestingLevel.Value > 0)
                        action();
                    else
                        Task.Run(() => NonNestedAction(action));
                    Started?.Invoke(this, EventArgs.Empty);
                    break;
                case CommandExecutionMode.AlwaysParallel:
                    Task.Run(() => NonNestedAction(action));
                    Started?.Invoke(this, EventArgs.Empty);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private CancellationTokenSource cancellationTokenSource;

        private void NonNestedAction(Action action)
        {
            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                Legacy.CancellationToken = cancellationTokenSource.Token;
                NestingLevel.Value += 1;
                action();
            }
            catch (OperationCanceledException)
            {
                Program.Console.Info($"Command {Name} cancelled.");
            }
            catch (Exception ex)
            {
                Program.Console.Error(ex.ToString());
            }
            finally
            {
                var source = cancellationTokenSource;
                cancellationTokenSource = null;
                source.Dispose();

                NestingLevel.Value -= 1;
                Stopped?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}