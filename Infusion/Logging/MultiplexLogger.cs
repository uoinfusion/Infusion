using System;

namespace Infusion.Logging
{
    public sealed class MultiplexLogger : ILogger
    {
        private readonly ILogger[] outputLoggers;

        public MultiplexLogger(params ILogger[] outputLoggers)
        {
            this.outputLoggers = outputLoggers;
        }

        public void ForeachLogger(Action<ILogger> loggerAction)
        {
            foreach (var logger in outputLoggers)
            {
                loggerAction(logger);
            }
        }

        public void Info(string message)
        {
            ForeachLogger(logger => logger.Info(message));
        }

        public void Important(string message)
        {
            ForeachLogger(logger => logger.Important(message));
        }

        public void Debug(string message)
        {
            ForeachLogger(logger => logger.Debug(message));
        }

        public void Critical(string message)
        {
            ForeachLogger(logger => logger.Critical(message));
        }

        public void Error(string message)
        {
            ForeachLogger(logger => logger.Error(message));
        }
    }
}
