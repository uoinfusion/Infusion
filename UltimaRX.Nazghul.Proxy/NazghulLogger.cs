using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using UltimaRX.Proxy.Logging;

namespace UltimaRX.Nazghul.Proxy
{
    internal sealed class NazghulLogger : ILogger
    {
        private readonly IHubProxy nazghulHub;

        public NazghulLogger(IHubProxy nazghulHub)
        {
            this.nazghulHub = nazghulHub;
        }

        public void WriteLine(string message)
        {
            nazghulHub.Invoke("SendLog", message);
        }
    }
}
