using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using Ultima;

namespace Infusion.Desktop.Launcher
{
    public class Launcher
    {
        private readonly IConsole console;

        public Launcher(IConsole console)
        {
            this.console = console;
        }

        public Task Launch(LauncherOptions options, InfusionProxy proxy)
        {
            return Task.Run(() =>
            {
                var serverEndPoint = options.ResolveServerEndpoint().Result;
                ushort proxyPort = GetProxyPort();

                CheckMulFiles(options);

                var proxyTask = proxy.Start(new ProxyStartConfig()
                {
                    ServerAddress = options.ServerEndpoint,
                    ServerEndPoint = serverEndPoint,
                    LocalProxyPort = proxyPort,
                    ProtocolVersion = options.ProtocolVersion,
                    Encryption = options.Encryption,
                    LoginEncryptionKey = options.Classic.EncryptionVersion?.Key
                });

                switch (options.ClientType)
                {
                    case UltimaClientType.Classic:
                        ClassicClientLauncher.Launch(console, proxy, options, proxyPort);
                        break;
                    case UltimaClientType.Orion:
                        OrionLauncher.Launch(console, proxy, options, proxyPort);
                        break;
                    case UltimaClientType.CrossUO:
                        CrossUOLauncher.Launch(console, proxy, options, proxyPort);
                        break;
                }

                InterProcessCommunication.StartReceiving();
            });
        }

        private void CheckMulFiles(LauncherOptions options)
        {
            bool requiresExplicitUltimaPath = false;
            if (string.IsNullOrEmpty(Files.RootDir))
            {
                console.Info("Cannot find Ultima Online installation.");
                requiresExplicitUltimaPath = true;
            }

            if (!string.IsNullOrEmpty(Files.RootDir) && !Directory.Exists(Files.RootDir))
            {
                console.Info($"Ultima Online installation path {Files.RootDir} doesn't exsist.");
                requiresExplicitUltimaPath = true;
            }

            if (requiresExplicitUltimaPath)
            {
                var clientDirectory = Path.GetDirectoryName(options.ClientExePath);
                console.Info($"Client path is {options.ClientExePath}. Assuming Ultima Online files are in {clientDirectory}.");
                Files.SetMulPath(clientDirectory);
            }

            console.Debug($"Loading mul files from {Files.RootDir}.");
        }

        private ushort GetProxyPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return (ushort)port;
        }
    }
}
