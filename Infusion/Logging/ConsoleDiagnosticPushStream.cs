using Infusion.Diagnostic;
using Infusion.Packets;

namespace Infusion.Logging
{
    internal sealed class ConsoleDiagnosticPushStream : TextDiagnosticPushStream
    {
        private readonly ILogger logger;

        public ConsoleDiagnosticPushStream(ILogger logger, string header, PacketDefinitionRegistry packetRegistry)
            : base(packetRegistry, header)
        {
            this.logger = logger;
        }

        protected override void OnPacketFinished()
        {
            logger.Debug(Flush());
        }
    }
}