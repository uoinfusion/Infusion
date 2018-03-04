using Infusion.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    public class DiagnosticTrace
    {
        private readonly ILogger logger;
        public bool Enabled { get; set; } = false;

        internal DiagnosticTrace(ILogger logger)
        {
            this.logger = logger;
        }

        [Conditional("DEBUG")]
        public void Log(string message)
        {
            if (Enabled)
                logger.Debug(message);
        }
    }
}
