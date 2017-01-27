using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace UltimaRX.Nazghul.WebServer
{
    [HubName("NazghulHub")]
    public class NazghulHub : Hub
    {
        public override async Task OnConnected()
        {
            await base.OnConnected();
        }

        public void SendLog(string message)
        {
            Clients.Others.SendLog(message);
        }

        public void RequestAllLogs()
        {
            Clients.Others.RequestAllLogs();
        }

        public void SendAllLogs(IEnumerable<string> messages)
        {
            Clients.Others.SendAllLogs(messages);
        }

        public void Say(string message)
        {
            Clients.Others.Say(message);
        }
    }
}