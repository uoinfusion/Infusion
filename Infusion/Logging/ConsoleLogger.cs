using System;

namespace Infusion.Logging
{
    internal sealed class ConsoleLogger : ILogger, ITimestampedLogger
    {
        public void WriteLine(DateTime timeStamp, string message)
        {
            Console.WriteLine($"{timeStamp} {message}");
        }

        public void Info(string message)
        {
            WriteLine(DateTime.Now, message);
        }

        public void Important(string message)
        {
            WriteLine(DateTime.Now, message);
        }

        public void Debug(string message)
        {
            WriteLine(DateTime.Now, message);
        }

        public void Critical(string message)
        {
            WriteLine(DateTime.Now, message);
        }

        public void Error(string message)
        {
            WriteLine(DateTime.Now, message);
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