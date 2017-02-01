using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;
using UltimaRX.Packets;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

namespace UltimaRX.Nazghul.Proxy
{
    public sealed class NazghulProxy : IDisposable
    {
        private readonly HubConnection hubConnection;
        private readonly IHubProxy nazghulHub;
        private readonly RingBufferLogger ringBufferLogger = new RingBufferLogger(64);

        public NazghulProxy(string hubUrl)
        {
            hubConnection = new HubConnection(hubUrl);
            nazghulHub = hubConnection.CreateHubProxy("NazghulHub");
            nazghulHub.On("RequestAllLogs", RequestAllLogs);
            nazghulHub.On<string>("Say", Say);
            hubConnection.Start().Wait();

            Program.Console = new MultiplexLogger(Program.Console, ringBufferLogger, new NazghulLogger(nazghulHub));
        }

        private void RequestAllLogs()
        {
            nazghulHub.Invoke("SendAllLogs", (IEnumerable<string>)ringBufferLogger.Dump());
        }

        public void Dispose()
        {
            hubConnection?.Dispose();
        }

        public void Say(string text)
        {
            if (Injection.CommandHandler.IsInvocationSyntax(text))
            {
                Injection.CommandHandler.Invoke(text);
            }
            else
            {
                Injection.Say(text);
            }
        }
    }
}
