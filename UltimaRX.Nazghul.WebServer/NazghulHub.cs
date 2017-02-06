using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using UltimaRX.Nazghul.Common;

namespace UltimaRX.Nazghul.WebServer
{
    [HubName("NazghulHub")]
    public class NazghulHub : Hub
    {
        public override async Task OnConnected()
        {
            await base.OnConnected();
        }

        public void SendLog(LogMessage message)
        {
            Clients.Others.SendLog(message);
        }

        public void RequestInitialInfo()
        {
            Clients.Others.RequestInitialInfo();
        }

        public void RequestAllLogs()
        {
            Clients.Others.RequestAllLogs();
        }

        public void SendAllLogs(IEnumerable<string> messages)
        {
            Clients.Others.SendAllLogs(messages);
        }

        public void RequestStatus()
        {
            Clients.Others.RequestStatus();
        }

        public void RequestScreenshot()
        {
            Clients.Others.RequestScreenshot();
        }

        public void ScreenshotReady()
        {
            Clients.All.ScreenshotReady();
        }

        public void SendStatus(PlayerStatus status)
        {
            Clients.Others.SendStatus(status);
        }

        public void Say(string message)
        {
            Clients.Others.Say(message);
        }
    }
}