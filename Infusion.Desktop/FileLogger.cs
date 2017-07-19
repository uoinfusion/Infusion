using System;
using System.IO;
using Infusion.Desktop.Profiles;
using Infusion.Logging;
using Infusion.Proxy;

namespace Infusion.Desktop
{
    internal sealed class FileLogger : ILogger
    {
        private readonly Configuration configuration;
        private object logLock = new object();

        public FileLogger(Configuration configuration)
        {
            this.configuration = configuration;
        }

        private void WriteLine(string message)
        {
            if (!configuration.LogToFileEnabled)
                return;

            try
            {
                lock (logLock)
                {
                    string logsPath = configuration.LogPath;

                    var now = DateTime.Now;

                    if (Directory.Exists(logsPath))
                    {
                        string fileName = Path.Combine(logsPath, $"{now:yyyy-MM-dd}.log");

                        if (!File.Exists(fileName))
                        {
                            File.Create(fileName).Dispose();
                        }

                        File.AppendAllText(fileName, $@"{now:HH:mm:ss:fffff}: {message}{Environment.NewLine}");
                    }
                }

            }
            catch (Exception)
            {
                // just swallow the exception, we don't want to terminate game because of logging problems
            }
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