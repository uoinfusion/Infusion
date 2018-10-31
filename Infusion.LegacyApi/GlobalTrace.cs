using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Infusion.LegacyApi.Console;
using Infusion.Logging;

namespace Infusion.LegacyApi
{
    public class GlobalTrace
    {
        private readonly ScriptTraceLogger logger;

        internal GlobalTrace(IConsole baseConsole)
        {
            logger = new ScriptTraceLogger(baseConsole);
            JournalTrace = new DiagnosticTrace(baseConsole);
        }

        public ScriptTrace Create() => new ScriptTrace(logger);

        public bool Enabled
        {
            get => logger.Enabled;
            set => logger.Enabled = value;
        }

        public void Log(string message)
        {
            logger.Debug(message);
        }

        public DiagnosticTrace JournalTrace { get; }
    }
}