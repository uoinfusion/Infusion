using System;

namespace Infusion.Logging
{
    internal sealed class MultiplexLogger : ITimestampedLogger
    {
        private readonly ITimestampedLogger[] outputLoggers;

        public MultiplexLogger(params ITimestampedLogger[] outputLoggers)
        {
            this.outputLoggers = outputLoggers;
        }

        public void ForeachLogger(Action<ITimestampedLogger> loggerAction)
        {
            foreach (var logger in outputLoggers)
            {
                loggerAction(logger);
            }
        }

        public void Info(DateTime timeStamp, string message)
        {
            ForeachLogger(logger => logger.Info(timeStamp, message));
        }

        public void Important(DateTime timeStamp, string message)
        {
            ForeachLogger(logger => logger.Important(timeStamp, message));
        }

        public void Debug(DateTime timeStamp, string message)
        {
            ForeachLogger(logger => logger.Debug(timeStamp, message));
        }

        public void Critical(DateTime timeStamp, string message)
        {
            ForeachLogger(logger => logger.Critical(timeStamp, message));
        }

        public void Error(DateTime timeStamp, string message)
        {
            ForeachLogger(logger => logger.Error(timeStamp, message));
        }
    }
}
