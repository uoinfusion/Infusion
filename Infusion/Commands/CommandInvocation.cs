using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Commands
{
    public sealed class CommandInvocation
    {
        internal CommandInvocation(Command command, string parameters, CommandExecutionMode mode, int nestingLevel,
            CancellationTokenSource cancellationTokenSource)
        {
            Command = command;
            Mode = mode;
            NestingLevel = nestingLevel;
            CancellationTokenSource = cancellationTokenSource;
            Parameters = parameters;
        }

        public string CommandName => Command.Name;

        public string Parameters { get; }

        public string Syntax => string.IsNullOrEmpty(Parameters) ? "," + CommandName : $",{CommandName} {Parameters}";

        public CommandExecutionMode Mode { get; }
        internal Command Command { get; }
        internal int NestingLevel { get; }
        internal bool Terminable => Command.Terminable;
        internal CancellationTokenSource CancellationTokenSource { get; }
        internal Task Task { get; set; }
    }
}