using Infusion.Diagnostic;
using Infusion.IO;

namespace Infusion.Proxy.Logging
{
    internal sealed class ConsoleDiagnosticPushStream : TextDiagnosticPushStream
    {
        private readonly ILogger logger;

        public ConsoleDiagnosticPushStream(ILogger logger, string header) : base(header)
        {
            this.logger = logger;
        }

        protected override void OnPacketFinished()
        {
            logger.Debug(Flush());
        }
    }
}