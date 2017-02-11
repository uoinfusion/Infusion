using System;
using System.Windows.Media;
using System.Windows.Threading;
using UltimaRX.Proxy;
using UltimaRX.Proxy.Logging;

namespace Infusion.Desktop
{
    public class InfusionConsoleLogger : ILogger
    {
        private readonly ConsoleContent consoleContent;
        private readonly Dispatcher dispatcher;

        public InfusionConsoleLogger(ConsoleContent consoleContent, Dispatcher dispatcher)
        {
            this.consoleContent = consoleContent;
            this.dispatcher = dispatcher;
        }

        private void WriteLine(string message, Brush textBrush)
        {
            dispatcher.BeginInvoke((Action) (() =>
            {
                consoleContent.Add($"{DateTime.Now} - {message}", textBrush);
            }));
        }

        public void Info(string message)
        {
            WriteLine(message, Brushes.Gray);
        }

        public void Speech(SpeechMessage message)
        {
            WriteLine(message.Text, Brushes.White);
        }

        public void Debug(string message)
        {
            WriteLine(message, Brushes.DimGray);
        }

        public void Critical(string message)
        {
            WriteLine(message, Brushes.Red);
        }

        public void Error(string message)
        {
            WriteLine(message, Brushes.DarkRed);
        }
    }
}