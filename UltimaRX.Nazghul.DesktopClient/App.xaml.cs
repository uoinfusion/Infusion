using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.AspNet.SignalR.Client;
using UltimaRX.Nazghul.Common;

namespace UltimaRX.Nazghul.DesktopClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 1 && e.Args[0].Equals("/command", StringComparison.OrdinalIgnoreCase))
            {
                var hubConnection = new HubConnection("http://localhost:9094/");
                var nazghulHub = hubConnection.CreateHubProxy("NazghulHub");
                hubConnection.Start().Wait();
                nazghulHub.Invoke<string>("Say", "," + e.Args[1]).Wait();
                hubConnection.Dispose();
                this.Shutdown(0);
            }
        }
    }
}
