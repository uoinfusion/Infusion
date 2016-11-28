using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.Logging
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
            logger.WriteLine(Flush());
        }
    }
}