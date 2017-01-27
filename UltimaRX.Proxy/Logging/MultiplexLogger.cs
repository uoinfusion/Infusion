using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Proxy.Logging
{
    public sealed class MultiplexLogger : ILogger
    {
        private readonly ILogger[] outputLoggers;

        public MultiplexLogger(params ILogger[] outputLoggers)
        {
            this.outputLoggers = outputLoggers;
        }

        public void WriteLine(string message)
        {
            foreach (var logger in outputLoggers)
            {
                logger.WriteLine(message);
            }
        }
    }
}
