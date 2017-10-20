using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Infusion.Logging;

namespace Infusion.Commands
{
    public sealed partial class CommandHandler
    {
        private readonly CommandInvocator invocator;
        private readonly ILogger logger;

        private readonly Dictionary<string, CommandInvocation> runningCommands =
            new Dictionary<string, CommandInvocation>();

        private readonly object runningCommandsLock = new object();
        private ImmutableDictionary<string, Command> commands = ImmutableDictionary<string, Command>.Empty;

        public CommandHandler(ILogger logger)
        {
            this.logger = logger;
            invocator = new CommandInvocator(this, logger);
        }

        public IEnumerable<string> CommandNames => commands.Keys;

        public Command[] RunningCommands
        {
            get
            {
                Command[] result;

                lock (runningCommandsLock)
                {
                    result = runningCommands.Values.Select(x => x.Command).ToArray();
                }

                return result;
            }
        }

        public event EventHandler<CommandInvocation> RunningCommandAdded;
        public event EventHandler<CommandInvocation> RunningCommandRemoved;
        public event EventHandler<CancellationToken> CancellationTokenCreated;

        public void OnCancellationTokenCreated(CancellationToken token)
        {
            CancellationTokenCreated?.Invoke(this, token);
        }

        private void RemoveCommandInvocation(CommandInvocation invocation)
        {
            var removed = false;

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
                var syntax = CommandParser.Parse(commandInvocationSyntax);
                if (!syntax.HasParameters)
                {
                    if (!commands.TryGetValue(syntax.Name, out Command command))
                        throw new CommandInvocationException($"Unknown command name '{commandInvocationSyntax}'");

                    invocator.Invoke(command, cancellationTokenSource);
                }
                else
                {
                    if (!commands.TryGetValue(syntax.Name, out Command command))
                        throw new CommandInvocationException($"Unknown command name '{commandInvocationSyntax}'");

                    if (command.AcceptsParameters)
                        invocator.Invoke(command, syntax.Parameters, cancellationTokenSource);
                    else
                        throw new CommandInvocationException($"Command '{command.Name}' doesn't accept parameters.");
                }
            }
            catch (CommandInvocationException ex)
            {
                logger.Error(ex.Message);
            }
        }

        private void CheckIfAlreadyRunning(Command command)
        {
            lock (runningCommandsLock)
            {
                if (runningCommands.ContainsKey(command.Name))
                    throw new CommandInvocationException($"Command '{command.Name}' is already running.");
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

        public void Terminate(bool force = false)
        {
            IEnumerable<CommandInvocation> invocations;

            lock (runningCommandsLock)
            {
                invocations = runningCommands.Values.Where(x => force || x.Mode != CommandExecutionMode.Background);
            }

            foreach (var invocation in invocations)
                invocation.CancellationTokenSource?.Cancel();
        }

        public string Help()
        {
            var help = new StringBuilder();

            help.AppendLine("Available commands:");
            foreach (var command in commands.Values.OrderBy(c => c.Name))
            {
                help.Append(",");
                help.Append(command.Name.PadRight(16));
                if (!string.IsNullOrEmpty(command.Summary))
                {
                    help.Append("\t");
                    help.Append(command.Summary);
                }
                help.AppendLine();
            }

            return help.ToString();
        }

        public string Help(string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
                return Help();

            if (commands.TryGetValue(commandName, out Command command))
            {
                var help = new StringBuilder();
                help.AppendLine();
                help.Append(",");
                help.Append(commandName);
                help.AppendLine();
                help.AppendLine();
                if (string.IsNullOrEmpty(command.Summary))
                    help.AppendLine("No command description");
                else
                    help.AppendLine(command.Summary);
                if (!string.IsNullOrEmpty(command.Description))
                {
                    if (!command.Summary.EndsWith("\n") && !command.Summary.EndsWith(Environment.NewLine))
                        help.AppendLine();
                    help.AppendLine();
                    help.AppendLine(command.Description);
                }

                return help.ToString();
            }
            return $"Unknown command '{commandName}'";
        }
    }
}