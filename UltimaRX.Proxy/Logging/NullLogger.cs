using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Proxy.Logging
{
    public class NullLogger : ILogger
    {
        public static ILogger Instance { get; } = new NullLogger();

        public void Info(string message)
        {
        }

        public void Speech(SpeechMessage message)
        {
        }

        public void Debug(string message)
        {
        }

        public void Critical(string message)
        {
        }

        public void Error(string message)
        {
        }
    }
}
