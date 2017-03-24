using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class CommandHandler
    {
        private ImmutableDictionary<string, Command> commands = ImmutableDictionary<string, Command>.Empty;

        public IEnumerable<string> CommandNames => commands.Keys;

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

        public void Invoke(string commandInvocationSyntax)
        {
            try
            {
                var firstSpaceIndex = commandInvocationSyntax.IndexOf(' ');
                if (firstSpaceIndex < 0)
                {
                    Command command;
                    string commandName = commandInvocationSyntax.Substring(1, commandInvocationSyntax.Length - 1);

                    if (!commands.TryGetValue(commandName, out command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    command.Invoke();
                }
                else
                {
                    Command command;

                    var commandName = commandInvocationSyntax.Substring(1, firstSpaceIndex - 1);

                    if (!commands.TryGetValue(commandName, out command))
                        throw new CommandInvocationException($"Unknown command name {commandInvocationSyntax}");

                    if (firstSpaceIndex + 1 >= commandInvocationSyntax.Length)
                        throw new CommandInvocationException(
                            $"No parameters for command specified {commandInvocationSyntax}");

                    var parameters = commandInvocationSyntax.Substring(firstSpaceIndex + 1,
                        commandInvocationSyntax.Length - firstSpaceIndex - 1);

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

        public bool IsInvocationSyntax(string potentialInvocationSyntax) => potentialInvocationSyntax.StartsWith(",");

        public void Unregister(string commandName)
        {
            commands = commands.Remove(commandName);
        }

        public void Unregister(Command command)
        {
            Unregister(command.Name);
        }
    }
}
