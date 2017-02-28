using System;
using System.Threading.Tasks;

namespace Infusion.Proxy.InjectionApi
{
    public sealed class Command
    {
        private readonly Action commandAction;
        private readonly CommandExecutionMode executionMode;
        private readonly Action<string> parameterizedCommandAction;

        public Command(string name, Action commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Script)
        {
            this.commandAction = commandAction;
            Name = name;
            this.executionMode = executionMode;
        }

        public Command(string name, Action<string> commandAction,
            CommandExecutionMode executionMode = CommandExecutionMode.Script)
        {
            Name = name;
            parameterizedCommandAction = commandAction;
            this.executionMode = executionMode;
        }

        public string Name { get; }

        public void Invoke()
        {
            Invoke(commandAction);
        }

        public void Invoke(string parameters)
        {
            Invoke(() => parameterizedCommandAction(parameters));
        }

        private void Invoke(Action action)
        {
            switch (executionMode)
            {
                case CommandExecutionMode.OwnThread:
                    Task.Run(action);
                    break;
                case CommandExecutionMode.Script:
                    Script.Run(action);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}