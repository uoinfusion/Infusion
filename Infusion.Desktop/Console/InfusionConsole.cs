using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using System;
using System.Threading.Tasks;

namespace Infusion.Desktop.Console
{
    internal class InfusionConsole : IConsole
    {
        private readonly FileConsole fileConsole;
        private readonly WpfConsole wpfConsole;
        private readonly object enqueueLock = new object();
        private Task lastTask;

        internal InfusionConsole(FileConsole fileConsole, WpfConsole wpfConsole)
        {
            this.fileConsole = fileConsole;
            this.wpfConsole = wpfConsole;
        }

        public void WriteSpeech(string name, string message, ObjectId? speakerId, Color color)
        {
            var now = DateTime.UtcNow;
            Enqueue(() =>
            {
                string text = !string.IsNullOrEmpty(name) ? $"{name}: {message}" : message;
                    wpfConsole.WriteJournalEntry(now, text, color);
                    fileConsole.WriteLine(now, text);
            });
        }


        public void WriteLine(ConsoleLineType type, string message)
        {
            var now = DateTime.UtcNow;

            Enqueue(() =>
            {
                wpfConsole.WriteLine(now, type, message);
                fileConsole.WriteLine(now, message);
            });
        }

        private void Enqueue(Action action)
        {
            lock (enqueueLock)
            {
                lastTask = lastTask?.ContinueWith(t => action())
                    ?? Task.Run(action);
            }
        }

        void ILogger.Info(string message)
        {
            WriteLine(ConsoleLineType.Information, message);
        }

        void ILogger.Important(string message)
        {
            WriteLine(ConsoleLineType.Important, message);
        }

        void ILogger.Debug(string message)
        {
            WriteLine(ConsoleLineType.Debug, message);
        }

        void ILogger.Critical(string message)
        {
            WriteLine(ConsoleLineType.Critical, message);
        }

        void ILogger.Error(string message)
        {
            WriteLine(ConsoleLineType.Error, message);
        }
    }
}
