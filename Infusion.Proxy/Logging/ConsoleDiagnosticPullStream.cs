using Infusion.IO;
using Infusion.Packets;

namespace Infusion.Proxy.Logging
{
    internal sealed class ConsoleDiagnosticPullStream : DiagnosticPullStream
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