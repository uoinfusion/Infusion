using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ultima;
using UltimaRX.Proxy;

namespace Infusion.Desktop.Launcher
{
    public static class Launcher
    {
        public static Task Launch(LauncherOptions options)
        {
            return Task.Run(() =>
            {
                var serverEndPoint = options.ResolveServerEndpoint().Result;

                var connectedToServerEvent = new AutoResetEvent(false);
                Program.ConnectedToServer += (sender, args) =>
                {
                    connectedToServerEvent.Set();
                };
                var proxyTask = Program.Start(serverEndPoint, options.ProxyPort);
                if (!connectedToServerEvent.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException("Server connection timeout.");
                }

                LoginConfiguration.SetServerAddress("127.0.0.1", options.ProxyPort);
                if (!string.IsNullOrEmpty(options.UserName))
                    UltimaConfiguration.SetUserName(options.UserName);
                if (!string.IsNullOrEmpty(options.Password))
                    UltimaConfiguration.SetPassword(options.EncryptPassword());

                string ultimaExecutablePath = Path.Combine(Files.RootDir, "NoCryptClient.exe");

                Process.Start(ultimaExecutablePath);

                InterProcessCommunication.StartReceiving();
            });
        }
    }
}
