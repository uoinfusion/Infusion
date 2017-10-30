using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Proxy;
using Ultima;

namespace Infusion.Desktop.Launcher
{
    public static class Launcher
    {
        public static Task Launch(LauncherOptions options)
        {
            return Task.Run(() =>
            {
                var serverEndPoint = options.ResolveServerEndpoint().Result;
                ushort proxyPort = options.ResolveProxyPort();

                var connectedToServerEvent = new AutoResetEvent(false);
                Program.ConnectedToServer += (sender, args) =>
                {
                    connectedToServerEvent.Set();
                };
                var proxyTask = Program.Start(serverEndPoint, proxyPort);
                if (!connectedToServerEvent.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException("Server connection timeout.");
                }

                LoginConfiguration.SetServerAddress("127.0.0.1", proxyPort);
                if (!string.IsNullOrEmpty(options.UserName))
                    UltimaConfiguration.SetUserName(options.UserName);
                if (!string.IsNullOrEmpty(options.Password))
                    UltimaConfiguration.SetPassword(options.EncryptPassword());

                string ultimaExecutablePath = Path.Combine(Files.RootDir, "NoCryptClient.exe");
                if (!File.Exists(ultimaExecutablePath))
                {
                    Program.Console.Error($"File {ultimaExecutablePath} doesn't exist.");
                    Program.Console.Info(
                        "Infusion requires that you use a client without encryption. If your Ultima Online server allows using Third Dawn (3.x) clients, " +
                        "you can download a client without encryption: https://ulozto.cz/!9w2rZmJfmcvA/client306m-patches-zip. \n\n" +
                        @"The zip file contains NoCryptClient.exe. Copy it to your Ultima Online installation folder (typically c:\Program Files\Ultima Online 2D)." +
                        "\n\n" +
                        "You can read more about how to setup Infusion properly: https://github.com/uoinfusion/Infusion/wiki/Getting-started." + "\n");

                    return;
                }

                Program.Console.Info($"Staring {ultimaExecutablePath}");

                var ultimaClientProcess = Process.Start(ultimaExecutablePath);
                if (ultimaClientProcess == null)
                {
                    Program.Console.Error($"Cannot start {ultimaExecutablePath}.");
                    return;
                }

                Program.SetClientWindowHandle(ultimaClientProcess);

                InterProcessCommunication.StartReceiving();
            });
        }
    }
}
