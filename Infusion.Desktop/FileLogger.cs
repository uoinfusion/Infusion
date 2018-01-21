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
                    var utcTimeStamp = timeStamp.ToUniversalTime();

                    string fileName = Path.Combine(logsPath, $"{utcTimeStamp:yyyy-MM-dd}.log");

                    bool createdNew = false;
                    if (!File.Exists(fileName))
                    {
                        File.Create(fileName).Dispose();
                        createdNew = true;
                    }

                    using (var stream =
                        new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            if (createdNew)
                            {
                                writer.WriteLine($"Log craeted on {utcTimeStamp.Date:d}, using UTC timestamps");
                                writer.WriteLine($@"Infusion {VersionHelpers.ProductVersion}");
                            }
                            writer.WriteLine($@"{utcTimeStamp:HH:mm:ss:fffff}: {message}");
                        }
                    }
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