using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Logging
{
    public class AsyncLogger : ILogger
    {
        private readonly ITimestampedLogger baseLogger;

        public AsyncLogger(ITimestampedLogger baseLogger)
        {
            this.baseLogger = baseLogger;
        }

        public void Info(string message)
        {
            var timeStamp = DateTime.Now;

            Enqueue(() => baseLogger.Info(timeStamp, message));
        }

        public void Important(string message)
        {
            var timeStamp = DateTime.Now;

            Enqueue(() => baseLogger.Important(timeStamp, message));
        }

        public void Debug(string message)
        {
            var timeStamp = DateTime.Now;

            Enqueue(() => baseLogger.Debug(timeStamp, message));
        }

        public void Critical(string message)
        {
            var timeStamp = DateTime.Now;

            Enqueue(() => baseLogger.Critical(timeStamp, message));
        }

        public void Error(string message)
        {
            var timeStamp = DateTime.Now;

            Enqueue(() => baseLogger.Error(timeStamp, message));
        }

        private readonly object enqueueLock = new object();
        private Task lastTask;

        private void Enqueue(Action action)
        {
            lock (enqueueLock)
            {
                lastTask = lastTask?.ContinueWith(t => action()) 
                    ?? Task.Run(action);
            }
        }
    }
}
