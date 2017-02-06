using UltimaRX.IO;

namespace UltimaRX.Proxy.Logging
{
    internal sealed class ConsoleDiagnosticPushStream : DiagnosticPushStream
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