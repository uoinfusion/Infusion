using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class Command
    {
        private readonly Action commandAction;
        public CommandExecutionMode ExecutionMode { get; }
        private readonly Action<string> parameterizedCommandAction;
        private static readonly ThreadLocal<int> nestingLevel = new ThreadLocal<int>();

        public Command(string name, Action commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            this.commandAction = commandAction;
            Name = name;
            this.ExecutionMode = executionMode;
        }

        public Command(string name, Action<string> commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Normal)
        {
            Name = name;
            parameterizedCommandAction = commandAction;
            this.ExecutionMode = executionMode;
        }

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
    }
}