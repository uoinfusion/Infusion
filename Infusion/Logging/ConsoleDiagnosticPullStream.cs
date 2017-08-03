using Infusion.Diagnostic;
using Infusion.Packets;

namespace Infusion.Logging
{
    internal sealed class ConsoleDiagnosticPullStream : TextDiagnosticPullStream
    {
        private readonly ILogger logger;

        public ConsoleDiagnosticPullStream(ILogger logger, string header) : base(header)
        {
            this.logger = logger;
        }

        protected override void OnPacketFinished(Packet packet)
        {
            logger.Debug(Flush());
        }
    }
}