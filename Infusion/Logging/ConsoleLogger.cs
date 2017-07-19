using System;

namespace Infusion.Logging
{
    public sealed class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
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