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

        public void WriteLine(string message)
        {
        }
    }
}
