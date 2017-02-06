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

        public string Format(string message)
            => $"{DateTime.UtcNow:hh:mm:ss.fff} - {Thread.CurrentThread.ManagedThreadId} - {message}";

        public void Info(string message)
        {
            baseLogger.Info(Format(message));
        }

        public void Speech(SpeechMessage message)
        {
            baseLogger.Speech(message);
        }

        public void Debug(string message)
        {
            baseLogger.Debug(Format(message));
        }

        public void Critical(string message)
        {
            baseLogger.Critical(Format(message));
        }

        public void Error(string message)
        {
            baseLogger.Error(Format(message));
        }
    }
}
