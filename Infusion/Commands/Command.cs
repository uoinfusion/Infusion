using System;
using System.Threading;

namespace Infusion.Commands
{
    public sealed class Command
    {
        private static readonly ThreadLocal<int> nestingLevel = new ThreadLocal<int>();
        private readonly Action commandAction;
        private readonly Action<string> parameterizedCommandAction;

        public Command(string name, Action commandAction, string summary = null, string description = null,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            this.commandAction = commandAction;
            Summary = summary;
            Description = description;
            Name = name;
            ExecutionMode = executionMode;
        }

        public Command(string name, Action<string> commandAction, string summary = null, string description = null,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            Name = name;
            parameterizedCommandAction = commandAction;
            Summary = summary;
            Description = description;
            ExecutionMode = executionMode;
        }

        public string Summary { get; }
        public string Description { get; }
        public CommandExecutionMode ExecutionMode { get; }

        public string Name { get; }

        internal void Invoke()
        {
            if (commandAction != null)
                commandAction();
            else
                parameterizedCommandAction?.Invoke(null);
        }

        internal void Invoke(string parameters)
        {
            parameterizedCommandAction(parameters);
        }

        internal bool AcceptsParameters
            => parameterizedCommandAction != null;
    }
}