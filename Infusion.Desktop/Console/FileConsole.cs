using Infusion.LegacyApi;
using Infusion.Utilities;
using System;
using System.IO;

namespace Infusion.Desktop.Console
{
    internal sealed class FileConsole : IDisposable
    {
        private readonly LogConfiguration logConfig;
        private readonly CircuitBreaker loggingBreaker;
        private readonly object logLock = new object();
        private bool firstWrite = true;
        private FileStream stream;
        private StreamWriter writer;
        private readonly RingBuffer<string> notLoggedMessages = new RingBuffer<string>(8192);

        public FileConsole(LogConfiguration logConfig, CircuitBreaker loggingBreaker)
        {
            this.logConfig = logConfig;
            this.loggingBreaker = loggingBreaker;
        }

        public void Dispose()
        {
            lock (logLock)
            {
                if (writer != null)
                {
                    writer.Dispose();
                    writer = null;
                }

                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        public void WriteLine(DateTime timeStamp, string message)
        {
            var logsPath = logConfig.LogPath;
            if (string.IsNullOrEmpty(logsPath) || !logConfig.LogToFileEnabled)
            {
                lock (logLock)
                {
                    notLoggedMessages.Add(FormatMessage(timeStamp, message));
                    return;
                }
            }

            loggingBreaker.Protect(() =>
            {
                lock (logLock)
                {
                    var fileName = Path.Combine(logsPath, $"{timeStamp:yyyy-MM-dd}.log");
                    logConfig.CurrentLogFile = fileName;

                    var createdNew = false;

                    if (!File.Exists(fileName))
                    {
                        if (!Directory.Exists(logsPath))
                            Directory.CreateDirectory(logsPath);

                        File.Create(fileName).Dispose();
                        createdNew = true;
                    }

                    if (stream == null || createdNew)
                    {
                        stream?.Dispose();
                        stream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
                        writer = new StreamWriter(stream);
                    }

                    if (firstWrite || createdNew)
                    {
                        if (TimeZone.CurrentTimeZone != null)
                        {
                            var utcHoursDiff = TimeZone.CurrentTimeZone.GetUtcOffset(timeStamp).TotalHours;
                            var utcHoursDiffStr = utcHoursDiff >= 0 ? $"+{utcHoursDiff}" : $"-{utcHoursDiff}";
                            writer.WriteLine(
                                $"Log created on {timeStamp.Date:d}, using {TimeZone.CurrentTimeZone.StandardName} timezone (UTC {utcHoursDiffStr} h)");
                        }
                        else
                        {
                            writer.WriteLine(
                                $"Log created on {timeStamp.Date:d}, unknown timezone");
                        }

                        firstWrite = false;
                    }

                    if (notLoggedMessages.Count > 0)
                    {
                        foreach (var msg in notLoggedMessages)
                            writer.WriteLine(msg);
                        notLoggedMessages.Clear();
                    }

                    writer.WriteLine(FormatMessage(timeStamp, message));
                    writer.Flush();
                }
            });
        }

        private string FormatMessage(DateTime timeStamp, string message)
            => $@"{timeStamp:HH:mm:ss:fffff}: {message}";
    }
}
