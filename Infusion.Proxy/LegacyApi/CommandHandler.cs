using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class CommandHandler
    {
        private readonly object runningCommandsLock = new object();
        private ImmutableDictionary<string, Command> commands = ImmutableDictionary<string, Command>.Empty;
        private readonly Dictionary<string, Command> runningCommands = new Dictionary<string, Command>();

        public IEnumerable<string> CommandNames => commands.Keys;

        public Command[] RunningCommands
        {
            get
            {
                Command[] result;

                lock (runningCommands)
                {
                    result = runningCommands.Values.ToArray();
                }

                return result;
            }
        }

        public event EventHandler<Command> CommandStopped;

        public Command RegisterCommand(string name, Action commandAction)
        {
            var command = new Command(name, commandAction);

            RegisterCommand(command);

            return command;
        }

        public void RegisterCommand(Command command)
        {
            command.Started += CommandOnStarted;
            command.Stopped += CommandOnStopped;
            commands = commands.SetItem(command.Name, command);
        }

        private void CommandOnStopped(object sender, EventArgs eventArgs)
        {
            var command = (Command) sender;
            lock (runningCommandsLock)
            {
                runningCommands.Remove(command.Name);
            }

            CommandStopped?.Invoke(this, command);
        }

        private void CommandOnStarted(object sender, EventArgs eventArgs)
        {
            var command = (Command) sender;
            lock (runningCommandsLock)
            {
                runningCommands.Add(command.Name, command);
            }
        }

        public Command RegisterCommand(string name, Action<string> commandAction)
        {
            var command = new Command(name, commandAction);

            RegisterCommand(command);

            return command;
        }

        public void Invoke(string commandInvocationSyntax)
        {
            try
            {
                var firstSpaceIndex = commandInvocationSyntax.IndexOf(' ');
                if (firstSpaceIndex < 0)
                {
                    Command command;
                    var commandName = commandInvocationSyntax.Substring(1, commandInvocationSyntax.Length - 1);

                    if (!commands.TryGetValue(commandName, out command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    CheckIfAlreadyRunning(command);
                    command.Invoke();
                }
                else
                {
                    Command command;

                    var commandName = commandInvocationSyntax.Substring(1, firstSpaceIndex - 1);

                    if (!commands.TryGetValue(commandName, out command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    if (firstSpaceIndex + 1 >= commandInvocationSyntax.Length)
                    {
                        throw new CommandInvocationException(
                            $"No parameters for command specified {commandInvocationSyntax}");
                    }

                    var parameters = commandInvocationSyntax.Substring(firstSpaceIndex + 1,
                        commandInvocationSyntax.Length - firstSpaceIndex - 1);

                    CheckIfAlreadyRunning(command);
                    command.Invoke(parameters);
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
            {
                command.Started -= CommandOnStarted;
                command.Stopped -= CommandOnStopped;

                commands = commands.Remove(commandName);
            }
        }

        public void Unregister(Command command)
        {
            Unregister(command.Name);
        }

        public void Terminate(string commandName)
        {
            if (commands.TryGetValue(commandName, out Command command))
                command.Terminate();
        }

        public void Terminate()
        {
            IEnumerable<Command> commands;

            lock (runningCommandsLock)
            {
                commands = runningCommands.Values;
            }

            foreach (var command in commands)
                command.Terminate();
        }
    }
}