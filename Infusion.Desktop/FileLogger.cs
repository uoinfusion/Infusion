using System;
using System.IO;
using Infusion.Desktop.Profiles;
using Infusion.Logging;
using Infusion.Proxy;
using Infusion.Utilities;

namespace Infusion.Desktop
{
    internal sealed class FileLogger : ITimestampedLogger
    {
        private readonly Configuration configuration;
        private readonly CircuitBreaker loggingBreaker;
        private object logLock = new object();

        public FileLogger(Configuration configuration, CircuitBreaker loggingBreaker)
        {
            this.configuration = configuration;
            this.loggingBreaker = loggingBreaker;
        }

        private void WriteLine(DateTime timeStamp, string message)
        {
            if (!configuration.LogToFileEnabled)
                return;

            loggingBreaker.Protect(() =>
            {
                lock (logLock)
                {
                    string logsPath = configuration.LogPath;

                    string fileName = Path.Combine(logsPath, $"{timeStamp:yyyy-MM-dd}.log");

                    if (!File.Exists(fileName))
                    {
                        File.Create(fileName).Dispose();
                        File.AppendAllText(fileName, $"Infusion {VersionHelpers.ProductVersion}");
                    }

                    File.AppendAllText(fileName, $@"{timeStamp:HH:mm:ss:fffff}: {message}{Environment.NewLine}");
                }
            });
        }

        public void Info(DateTime timeStamp, string message)
        {
            WriteLine(timeStamp, message);
        }

        public void Important(DateTime timeStamp, string message)
        {
            WriteLine(timeStamp, message);
        }

        public void Debug(DateTime timeStamp, string message)
        {
            WriteLine(timeStamp, message);
        }

        public void Critical(DateTime timeStamp, string message)
        {
            WriteLine(timeStamp, message);
        }

        public void Error(DateTime timeStamp, string message)
        {
            WriteLine(timeStamp, message);
        }
    }
}