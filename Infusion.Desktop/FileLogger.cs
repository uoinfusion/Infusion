using System;
using System.IO;
using Infusion.Desktop.Profiles;
using Infusion.Proxy;
using Infusion.Proxy.Logging;

namespace Infusion.Desktop
{
    internal sealed class FileLogger : ILogger
    {
        private object logLock = new object();

        private void WriteLine(string message)
        {
            try
            {
                lock (logLock)
                {
                    if (!ProfileRepositiory.SelectedProfile.Options.LogToFileEnabled)
                        return;

                    string logsPath = ProfileRepositiory.SelectedProfile.Options.LogPath;

                    var now = DateTime.Now;

                    if (Directory.Exists(logsPath))
                    {
                        string fileName = Path.Combine(logsPath, $"{now:yyyy-MM-dd}.log");

                        if (!File.Exists(fileName))
                        {
                            File.Create(fileName).Dispose();
                        }

                        File.AppendAllText(fileName, $@"{now:T}: {message}{Environment.NewLine}");
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