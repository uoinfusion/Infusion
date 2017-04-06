using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public sealed partial class CommandHandler
    {
        private readonly CommandInvocator invocator;

        private readonly Dictionary<string, CommandInvocation> runningCommands =
            new Dictionary<string, CommandInvocation>();

        private readonly object runningCommandsLock = new object();
        private ImmutableDictionary<string, Command> commands = ImmutableDictionary<string, Command>.Empty;

        public event EventHandler<CommandInvocation> RunningCommandAdded;
        public event EventHandler<CommandInvocation> RunningCommandRemoved;

        private void RemoveCommandInvocation(CommandInvocation invocation)
        {
            bool removed = false;

            lock (runningCommandsLock)
            {
                if (runningCommands.ContainsKey(invocation.Command.Name))
                {
                    runningCommands.Remove(invocation.Command.Name);
                    removed = true;
                }
            }

            if (removed)
                RunningCommandRemoved?.Invoke(this, invocation);
        }

        private void AddCommandInvocation(CommandInvocation invocation)
        {
            lock (runningCommandsLock)
            {
                runningCommands.Add(invocation.Command.Name, invocation);
            }

            RunningCommandAdded?.Invoke(this, invocation);
        }


        public CommandHandler()
        {
            invocator = new CommandInvocator(this);
        }

        public IEnumerable<string> CommandNames => commands.Keys;

        public Command[] RunningCommands
        {
            get
            {
                Command[] result;

                lock (runningCommands)
                {
                    result = runningCommands.Values.Select(x => x.Command).ToArray();
                }

                return result;
            }
        }

        public Command RegisterCommand(string name, Action commandAction)
        {
            var command = new Command(name, commandAction);

            RegisterCommand(command);

            return command;
        }

        public void RegisterCommand(Command command)
        {
            commands = commands.SetItem(command.Name, command);
        }

        public Command RegisterCommand(string name, Action<string> commandAction)
        {
            var command = new Command(name, commandAction);

            RegisterCommand(command);

            return command;
        }

        public void Invoke(string commandInvocationSyntax, CancellationTokenSource cancellationTokenSource = null)
        {
            try
            {
                var firstSpaceIndex = commandInvocationSyntax.IndexOf(' ');
                if (firstSpaceIndex < 0)
                {
                    var commandName = commandInvocationSyntax.Substring(1, commandInvocationSyntax.Length - 1);

                    if (!commands.TryGetValue(commandName, out Command command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    invocator.Invoke(command, cancellationTokenSource);
                }
                else
                {

                    var commandName = commandInvocationSyntax.Substring(1, firstSpaceIndex - 1);

                    if (!commands.TryGetValue(commandName, out Command command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    if (firstSpaceIndex + 1 >= commandInvocationSyntax.Length)
                    {
                        throw new CommandInvocationException(
                            $"No parameters for command specified {commandInvocationSyntax}");
                    }

                    var parameters = commandInvocationSyntax.Substring(firstSpaceIndex + 1,
                        commandInvocationSyntax.Length - firstSpaceIndex - 1);

                    invocator.Invoke(command, parameters, cancellationTokenSource);
                }
            }
            catch (CommandInvocationException ex)
            {
                Program.Console.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Program.Console.Error(ex.ToString());
            }
        }

        private void CheckIfAlreadyRunning(Command command)
        {
            lock (runningCommandsLock)
            {
                if (runningCommands.ContainsKey(command.Name))
                    throw new CommandInvocationException($"Command {command.Name} is already running.");
            }
        }

        public bool IsInvocationSyntax(string potentialInvocationSyntax) => potentialInvocationSyntax.StartsWith(",");

        public void Unregister(string commandName)
        {
            if (commands.TryGetValue(commandName, out Command command))
                commands = commands.Remove(commandName);
        }

        public void Unregister(Command command)
        {
            Unregister(command.Name);
        }

        public void Terminate(string commandName)
        {
            lock (runningCommandsLock)
            {
                if (runningCommands.TryGetValue(commandName, out CommandInvocation invocation))
                    invocation.CancellationTokenSource?.Cancel();
            }
        }

        public void Terminate()
        {
            IEnumerable<CommandInvocation> invocations;

            lock (runningCommandsLock)
            {
                invocations = runningCommands.Values;
            }

            foreach (var invocation in invocations)
                invocation.CancellationTokenSource?.Cancel();
        }
    }
}