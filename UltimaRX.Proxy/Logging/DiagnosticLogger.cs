using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UltimaRX.Proxy.Logging
{
    public class DiagnosticLogger : ILogger
    {
        private readonly ILogger baseLogger;

        public DiagnosticLogger(ILogger baseLogger)
        {
            this.baseLogger = baseLogger;
        }

        public void WriteLine(string message)
        {
            baseLogger.WriteLine($"{DateTime.UtcNow:hh:mm:ss.fff} - {Thread.CurrentThread.ManagedThreadId} - {message}");
        }
    }
}
