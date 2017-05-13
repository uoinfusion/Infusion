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

            public void Invoke(Command command, CancellationTokenSource cancellationTokenSource)
            {
                commandHandler.CheckIfAlreadyRunning(command);
                InvokeCore(command.Invoke, command, string.Empty, cancellationTokenSource);
            }

            internal void Invoke(Command command, string parameters, CancellationTokenSource cancellationTokenSource)
            {
                commandHandler.CheckIfAlreadyRunning(command);
                InvokeCore(() => command.Invoke(parameters), command, parameters, cancellationTokenSource);
            }

            private void InvokeCore(Action action, Command command, string parameters, CancellationTokenSource cancellationTokenSource)
            {
                switch (command.ExecutionMode)
                {
                    case CommandExecutionMode.Direct:
                        NestedAction(action, command, parameters, cancellationTokenSource);
                        break;
                    case CommandExecutionMode.Normal:
                        if (nestingLevel.Value > 0)
                            NestedAction(action, command, parameters, cancellationTokenSource);
                        else
                            Task.Run(() => NonNestedAction(action, parameters, command, cancellationTokenSource));
                        break;
                    case CommandExecutionMode.AlwaysParallel:
                        Task.Run(() => NonNestedAction(action, parameters, command, cancellationTokenSource));
                        break;
                    default:
                        throw new InvalidOperationException();
                }
            }

            private void NestedAction(Action action, Command command, string parameters, CancellationTokenSource cancellationTokenSource)
            {
                var invocation = new CommandInvocation(command, parameters, command.ExecutionMode, nestingLevel.Value,
                    cancellationTokenSource);
                var commandAdded = AddCommandInvocation(command, invocation);

                if (cancellationTokenSource != null)
                    Legacy.CancellationToken = cancellationTokenSource.Token;

                nestingLevel.Value += 1;
                try
                {
                    action();
                }
                catch (OperationCanceledException)
                {
                    Program.Console.Info($"Command {command.Name} cancelled.");
                }
                finally
                {
                    nestingLevel.Value -= 1;
                    if (commandAdded)
                        commandHandler.RemoveCommandInvocation(invocation);
                }
            }

            private void NonNestedAction(Action action, string parameters, Command command, CancellationTokenSource cancellationTokenSource)
            {
                cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();
                Legacy.CancellationToken = cancellationTokenSource.Token;
                var commandInvocation = new CommandInvocation(command, parameters, command.ExecutionMode, nestingLevel.Value,
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
                    cancellationTokenSource.Dispose();

                    nestingLevel.Value -= 1;
                }
            }

            private bool AddCommandInvocation(Command command, CommandInvocation commandInvocation)
            {
                if (command.ExecutionMode == CommandExecutionMode.AlwaysParallel || nestingLevel.Value == 0)
                {
                    commandHandler.AddCommandInvocation(commandInvocation);
                    return true;
                }

                return false;
            }
        }
    }
}