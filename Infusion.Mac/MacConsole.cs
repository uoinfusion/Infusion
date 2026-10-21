using System;
using System.IO;
using System.Threading.Tasks;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Utilities;

namespace Infusion.Mac
{
    internal enum ConsoleFilter
    {
        All,
        NoDebug,
        Game,
        SpeechOnly,
    }

    /// <summary>Infusion console in the terminal (ANSI colors) + log files, same as on Windows.</summary>
    internal sealed class MacConsole : IConsole
    {
        private readonly FileConsole fileConsole;
        private readonly object enqueueLock = new object();
        private Task lastTask;

        public ConsoleFilter Filter { get; set; } = ConsoleFilter.NoDebug;

        public MacConsole(FileConsole fileConsole)
        {
            this.fileConsole = fileConsole;
        }

        public void WriteSpeech(string name, string message, ObjectId? speakerId, Color color, ModelId bodyType, SpeechType type)
        {
            var now = DateTime.UtcNow;
            Enqueue(() =>
            {
                string text = !string.IsNullOrEmpty(name) ? $"{name}: {message}" : message;
                Print(now, "\u001b[38;5;180m", text);
                fileConsole?.WriteLine(now, text);
            });
        }

        public void WriteLine(ConsoleLineType type, string message)
        {
            var now = DateTime.UtcNow;
            Enqueue(() =>
            {
                if (IsVisible(type))
                    Print(now, ColorOf(type), message);
                fileConsole?.WriteLine(now, message);
            });
        }

        private bool IsVisible(ConsoleLineType type)
        {
            switch (Filter)
            {
                case ConsoleFilter.All:
                    return true;
                case ConsoleFilter.NoDebug:
                    return type != ConsoleLineType.Debug;
                case ConsoleFilter.Game:
                    return type != ConsoleLineType.Debug && type != ConsoleLineType.ScriptEcho;
                case ConsoleFilter.SpeechOnly:
                    return type == ConsoleLineType.Error || type == ConsoleLineType.Critical;
                default:
                    return true;
            }
        }

        private static string ColorOf(ConsoleLineType type)
        {
            switch (type)
            {
                case ConsoleLineType.Debug: return "\u001b[90m";
                case ConsoleLineType.Warning: return "\u001b[33m";
                case ConsoleLineType.Error: return "\u001b[31m";
                case ConsoleLineType.Critical: return "\u001b[1;31m";
                case ConsoleLineType.Important: return "\u001b[1;33m";
                case ConsoleLineType.ScriptResult: return "\u001b[36m";
                case ConsoleLineType.ScriptEcho: return "\u001b[90m";
                case ConsoleLineType.SkillChanged: return "\u001b[32m";
                default: return "\u001b[0m";
            }
        }

        private static readonly object printLock = new object();

        private static void Print(DateTime utc, string color, string message)
        {
            lock (printLock)
            {
                var local = utc.ToLocalTime();
                System.Console.Out.WriteLine($"\u001b[90m{local:HH:mm:ss}\u001b[0m {color}{message}\u001b[0m");
                System.Console.Out.Flush();
            }
        }

        public void Clear()
        {
            lock (printLock)
            {
                System.Console.Out.Write("\u001b[2J\u001b[H");
            }
        }

        private void Enqueue(Action action)
        {
            lock (enqueueLock)
            {
                lastTask = lastTask?.ContinueWith(t => action()) ?? Task.Run(action);
            }
        }

        public void Info(string message) => WriteLine(ConsoleLineType.Information, message);
        public void Important(string message) => WriteLine(ConsoleLineType.Important, message);
        public void Debug(string message) => WriteLine(ConsoleLineType.Debug, message);
        public void Critical(string message) => WriteLine(ConsoleLineType.Critical, message);
        public void Error(string message) => WriteLine(ConsoleLineType.Error, message);
    }

    /// <summary>Copy of FileConsole from Infusion.Desktop (logs to &lt;root&gt;/logs/...).</summary>
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
                writer?.Dispose();
                writer = null;
                stream?.Dispose();
                stream = null;
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
                        var utcHoursDiff = TimeZoneInfo.Local.GetUtcOffset(timeStamp).TotalHours;
                        var utcHoursDiffStr = utcHoursDiff >= 0 ? $"+{utcHoursDiff}" : $"{utcHoursDiff}";
                        writer.WriteLine($"Log created on {timeStamp.Date:d}, using {TimeZoneInfo.Local.StandardName} timezone (UTC {utcHoursDiffStr} h)");
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

        private static string FormatMessage(DateTime timeStamp, string message)
            => $@"{timeStamp:HH:mm:ss:fffff}: {message}";
    }
}
