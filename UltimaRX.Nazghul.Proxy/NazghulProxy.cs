using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNet.SignalR.Client;
using UltimaRX.Nazghul.Common;
using UltimaRX.Packets;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

namespace UltimaRX.Nazghul.Proxy
{
    public sealed class NazghulProxy : IDisposable
    {
        private readonly HubConnection hubConnection;
        private readonly string nazghulApiUrl;
        private readonly IHubProxy nazghulHub;
        private readonly RingBufferLogger ringBufferLogger = new RingBufferLogger(64);

        private DateTime lastLocationChanged;

        public NazghulProxy(string nazghulApiUrl)
        {
            this.nazghulApiUrl = nazghulApiUrl;
            hubConnection = new HubConnection(nazghulApiUrl);
            nazghulHub = hubConnection.CreateHubProxy("NazghulHub");
            nazghulHub.On("RequestInitialInfo", RequestInitialInfo);
            nazghulHub.On("RequestAllLogs", RequestAllLogs);
            nazghulHub.On("ReqeustStatus", RequestStatus);
            nazghulHub.On("RequestScreenshot", RequestScreenshot);
            nazghulHub.On<string>("Say", Say);
            hubConnection.Start().Wait();

            Program.Console = new MultiplexLogger(Program.Console, ringBufferLogger, new NazghulLogger(this));

            Injection.Me.LocationChanged += OnLocationChanged;

            RequestAllLogs();
            RequestStatus();
        }

        public void Dispose()
        {
            hubConnection?.Dispose();
        }

        public void RequestScreenshot()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(nazghulApiUrl);
                using (var content = new MultipartFormDataContent())
                {
                    byte[] contentBytes;
                    using (var memoryStream = new MemoryStream())
                    {
                        ScreenshotHelpers.TakeScreenshot("NoCryptClient", memoryStream);
                        contentBytes = memoryStream.ToArray();
                    }

                    var fileContent = new ByteArrayContent(contentBytes);

                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = "image.jpg"
                    };
                    content.Add(fileContent);

                    var requestUri = "/api/screenshot";
                    var result = client.PostAsync(requestUri, content).Result;
                }
            }
        }

        private void OnLocationChanged(object sender, Location3D location3D)
        {
            var currentTime = DateTime.UtcNow;
            if (lastLocationChanged.AddSeconds(1) < currentTime)
            {
                lastLocationChanged = currentTime;
                RequestStatus();
            }
        }

        private void RequestInitialInfo()
        {
            RequestStatus();
            RequestAllLogs();
        }

        private void RequestStatus()
        {
            nazghulHub.Invoke("SendStatus", new PlayerStatus
            {
                XLoc = Injection.Me.Location.X,
                YLoc = Injection.Me.Location.Y,
                CurrentHealth = Injection.Me.CurrentHealth,
                CurrentStamina = Injection.Me.CurrentStamina,
                Weight = Injection.Me.Weight
            });
        }

        private void RequestAllLogs()
        {
            nazghulHub.Invoke("SendAllLogs", (IEnumerable<string>) ringBufferLogger.Dump());
        }

        public void Say(string text)
        {
            if (Injection.CommandHandler.IsInvocationSyntax(text))
                Injection.CommandHandler.Invoke(text);
            else
                Injection.Say(text);
        }

        public void SendLog(string message)
        {
            SendLog(new LogMessage
            {
                Type = LogMessageType.Info,
                Message = message
            });
        }

        public void SendLog(LogMessage message)
        {
            nazghulHub.Invoke("SendLog", message);
        }
    }
}