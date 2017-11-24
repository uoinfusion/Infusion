using System;
using System.IO;
using Infusion.Desktop.Profiles;
using Infusion.Logging;
using Infusion.Proxy;
using Infusion.Utilities;

namespace Infusion.Desktop
{
    internal sealed class FileLogger : ILogger
    {
        private readonly Configuration configuration;
        private readonly CircuitBreaker loggingBreaker;
        private object logLock = new object();

        public FileLogger(Configuration configuration, CircuitBreaker loggingBreaker)
        {
            this.configuration = configuration;
            this.loggingBreaker = loggingBreaker;
        }

        private void WriteLine(string message)
        {
            if (!configuration.LogToFileEnabled)
                return;

            loggingBreaker.Protect(() =>
            {
                lock (logLock)
                {
                    string logsPath = configuration.LogPath;

                    var now = DateTime.Now;

                    string fileName = Path.Combine(logsPath, $"{now:yyyy-MM-dd}.log");

                    if (!File.Exists(fileName))
                    {
                        File.Create(fileName).Dispose();
                    }

                    File.AppendAllText(fileName, $@"{now:HH:mm:ss:fffff}: {message}{Environment.NewLine}");
                }
            });
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void Important(string message)
        {
            WriteLine(message);
        }

        public void Debug(string message)
        {
            WriteLine(message);
        }

        public void Critical(string message)
        {
            WriteLine(message);
        }

        public void Error(string message)
        {
            WriteLine(message);
        }
    }
}