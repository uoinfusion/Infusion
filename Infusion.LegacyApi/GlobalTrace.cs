using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Infusion.Logging;

namespace Infusion.LegacyApi
{
    public class GlobalTrace
    {
        private readonly ScriptTraceLogger logger;

        internal GlobalTrace(ILogger baseLogger)
        {
            logger = new ScriptTraceLogger(baseLogger);
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
    }
}