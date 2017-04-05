using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy.LegacyApi
{
    partial class CommandHandler
    {
        private sealed class CommandInvocator
        {
            private readonly CommandHandler commandHandler;
            private readonly ThreadLocal<int> nestingLevel = new ThreadLocal<int>();

            public CommandInvocator(CommandHandler commandHandler)
            {
                this.commandHandler = commandHandler;
            }

            public void Invoke(Command command)
            {
                commandHandler.CheckIfAlreadyRunning(command);
                InvokeCore(command.Invoke, command);
            }

            internal void Invoke(Command command, string parameters)
            {
                commandHandler.CheckIfAlreadyRunning(command);
                InvokeCore(() => command.Invoke(parameters), command);
            }

            private void InvokeCore(Action action, Command command)
            {
                switch (command.ExecutionMode)
                {
                    case CommandExecutionMode.Direct:
                        NestedAction(action, command);
                        break;
                    case CommandExecutionMode.Normal:
                        if (nestingLevel.Value > 0)
                            NestedAction(action, command);
                        else
                            Task.Run(() => NonNestedAction(action, command));
                        break;
                    case CommandExecutionMode.AlwaysParallel:
                        Task.Run(() => NonNestedAction(action, command));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private void NestedAction(Action action, Command command)
            {
                AddCommandInvocation(command,
                    new CommandInvocation(command, command.ExecutionMode, nestingLevel.Value, null));
                nestingLevel.Value += 1;
                try
                {
                    action();
                }
                finally
                {
                    nestingLevel.Value -= 1;
                }
            }

            private void NonNestedAction(Action action, Command command)
            {
                var cancellationTokenSource = new CancellationTokenSource();
                Legacy.CancellationToken = cancellationTokenSource.Token;
                var commandInvocation = new CommandInvocation(command, command.ExecutionMode, nestingLevel.Value,
                    cancellationTokenSource);

                AddCommandInvocation(command, commandInvocation);
                nestingLevel.Value += 1;

                try
                {
                    action();
                }
                catch (OperationCanceledException)
                {
                    Program.Console.Info($"Command {command.Name} cancelled.");
                }
                catch (Exception ex)
                {
                    Program.Console.Error(ex.ToString());
                }
                finally
                {
                    commandHandler.RemoveCommandInvocation(commandInvocation);
                    var source = cancellationTokenSource;
                    cancellationTokenSource = null;
                    source.Dispose();

                    nestingLevel.Value -= 1;
                }
            }

            private void AddCommandInvocation(Command command, CommandInvocation commandInvocation)
            {
                if (command.ExecutionMode == CommandExecutionMode.AlwaysParallel || nestingLevel.Value == 0)
                    commandHandler.AddCommandInvocation(commandInvocation);
            }
        }
    }
}