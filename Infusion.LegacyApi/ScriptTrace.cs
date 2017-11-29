using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Logging;

namespace Infusion.LegacyApi
{
    public class ScriptTrace
    {
        private readonly ILogger logger;
        public bool Enabled { get; set; } = false;

        internal ScriptTrace(ILogger logger)
        {
            this.logger = logger;
        }

        public void Log(string message)
        {
            if (Enabled)
                logger.Debug(message);
        }
    }
}
