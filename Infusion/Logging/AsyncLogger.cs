using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Logging
{
    public class AsyncLogger : ILogger
    {
        private readonly ILogger baseLogger;

        public AsyncLogger(ILogger baseLogger)
        {
            this.baseLogger = baseLogger;
        }

        public void Info(string message)
        {
            Task.Run(() => baseLogger.Info(message));
        }

        public void Important(string message)
        {
            Task.Run(() => baseLogger.Important(message));
        }

        public void Debug(string message)
        {
            Task.Run(() => baseLogger.Debug(message));
        }

        public void Critical(string message)
        {
            Task.Run(() => baseLogger.Critical(message));
        }

        public void Error(string message)
        {
            Task.Run(() => baseLogger.Error(message));
        }
    }
}
