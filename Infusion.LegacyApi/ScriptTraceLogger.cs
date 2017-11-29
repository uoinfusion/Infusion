using Infusion.Logging;

namespace Infusion.LegacyApi
{
    internal class ScriptTraceLogger : ILogger
    {
        private readonly ILogger baseLogger;

        internal ScriptTraceLogger(ILogger baseLogger)
        {
            this.baseLogger = baseLogger;
        }

        public bool Enabled { get; set; }

        public void Info(string message)
        {
            if (Enabled)
                baseLogger.Info(message);
        }

        public void Important(string message)
        {
            if (Enabled)
                baseLogger.Info(message);
        }

        public void Debug(string message)
        {
            if (Enabled)
                baseLogger.Info(message);
        }

        public void Critical(string message)
        {
            if (Enabled)
                baseLogger.Info(message);
        }

        public void Error(string message)
        {
            if (Enabled)
                baseLogger.Info(message);
        }
    }
}