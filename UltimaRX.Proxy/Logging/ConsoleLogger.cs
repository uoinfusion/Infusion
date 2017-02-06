using System;

namespace UltimaRX.Proxy.Logging
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }

        public void Info(string message)
        {
            WriteLine(message);
        }

        public void Speech(SpeechMessage message)
        {
            WriteLine(message.Text);
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