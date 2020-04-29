using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        public IEnumerable<string> CommandNames => commands.Keys.Select(k => CommandPrefix + k);
        public string CommandPrefix { get; set; } = ",";

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

        internal event EventHandler<CommandInvocation> RunningCommandAdded;
        internal event EventHandler<CommandInvocation> RunningCommandRemoved;
        internal event EventHandler<CancellationToken> CancellationTokenCreated;

        public bool TryGetCommand(string name, out Command command) =>
            this.commands.TryGetValue(name, out command);

        internal void OnCancellationTokenCreated(CancellationToken token)
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

        public void Invoke(string commandName, CommandExecutionMode? mode = null, CancellationTokenSource cancellationTokenSource = null)
        {
            try
            { 
                if (!commands.TryGetValue(commandName, out Command command))
                    throw new CommandInvocationException($"Unknown command name '{commandName}'");

                invocator.Invoke(command, mode, cancellationTokenSource);
            }
            catch (CommandInvocationException ex)
            {
                logger.Error(ex.Message);
            }

        }

        public void Invoke(string commandName, string commandParameters, CommandExecutionMode? mode = null,
            CancellationTokenSource cancellationTokenSource = null)
        {
            try
            {
                if (!commands.TryGetValue(commandName, out Command command))
                    throw new CommandInvocationException($"Unknown command name '{commandName}'");

                if (command.AcceptsParameters)
                    invocator.Invoke(command, commandParameters, mode, cancellationTokenSource);
                else
                    throw new CommandInvocationException($"Command '{command.Name}' doesn't accept parameters.");
            }
            catch (CommandInvocationException ex)
            {
                logger.Error(ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Debug(ex.ToString());
            }
        }

        public void InvokeSyntax(string commandInvocationSyntax, CommandExecutionMode? mode = null, CancellationTokenSource cancellationTokenSource = null)
        {
            var syntax = CommandParser.Parse(commandInvocationSyntax);
            if (!syntax.HasParameters)
            {
                Invoke(syntax.Name, mode, cancellationTokenSource);
            }
            else
            {
                Invoke(syntax.Name, syntax.Parameters, mode, cancellationTokenSource);
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

        public bool IsInvocationSyntax(string potentialInvocationSyntax) => potentialInvocationSyntax.StartsWith(CommandPrefix);

        public void Unregister(string commandName)
        {
            if (commands.TryGetValue(commandName, out Command command))
                commands = commands.Remove(commandName);
        }

        internal void UnregisterAllPublic()
        {
            var publicCommands = commands.Where(x => !x.Value.BuiltIn).Select(x => x.Key).ToArray();
            commands = commands.RemoveRange(publicCommands);
        }

        public void Unregister(Command command)
        {
            Unregister(command.Name);
        }

        public void Terminate(string commandName, TimeSpan? timeout = null)
        {
            CommandInvocation awaitedInvocation = null;

            lock (runningCommandsLock)
            {
                if (runningCommands.TryGetValue(commandName, out CommandInvocation invocation))
                {
                    if (!invocation.Terminable)
                        return;

                    awaitedInvocation = invocation;
                    invocation.CancellationTokenSource?.Cancel();
                }
                else
                {
                    return;
                }
            }

            if (awaitedInvocation != null)
            {
                if (!awaitedInvocation.Task.Wait(timeout ?? TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException($"Cannot terminate command {awaitedInvocation.CommandName}");
                }
            }
        }

        public void BeginTerminate(bool force = false)
        {
            IEnumerable<CommandInvocation> invocations;

            lock (runningCommandsLock)
            {
                invocations = runningCommands.Values.Where(x => x.Terminable && (force || x.Mode != CommandExecutionMode.Background));
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
                help.Append(CommandPrefix);
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

        public bool IsCommandRunning(string commandName) => 
            RunningCommands.Any(x => x.Name.Equals(commandName, StringComparison.Ordinal));
    }
}