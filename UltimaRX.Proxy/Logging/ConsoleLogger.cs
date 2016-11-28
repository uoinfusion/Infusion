using System;

namespace UltimaRX.Proxy.Logging
{
    internal sealed class ConsoleLogger : ILogger
    {
        public void WriteLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}