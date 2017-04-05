using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class CommandInvocation
    {
        internal CommandInvocation(Command command, CommandExecutionMode mode, int nestingLevel,
            CancellationTokenSource cancellationTokenSource)
        {
            Command = command;
            Mode = mode;
            NestingLevel = nestingLevel;
            CancellationTokenSource = cancellationTokenSource;
        }

        public string CommandName => Command.Name;

        public CommandExecutionMode Mode { get; }
        internal Command Command { get; }
        internal int NestingLevel { get; }
        internal CancellationTokenSource CancellationTokenSource { get; }
    }
}