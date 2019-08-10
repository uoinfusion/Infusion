using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
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
                ushort proxyPort = GetProxyPort();

                CheckMulFiles(options);

                var proxyTask = InfusionProxy.Start(new ProxyStartConfig()
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
                        ClassicClientLauncher.Launch(options, proxyPort);
                        break;
                    case UltimaClientType.Orion:
                        OrionLauncher.Launch(options, proxyPort);
                        break;
                    case UltimaClientType.CrossUO:
                        CrossUOLauncher.Launch(options, proxyPort);
                        break;
                }

                InterProcessCommunication.StartReceiving();
            });
        }

        private static void CheckMulFiles(LauncherOptions options)
        {
            bool requiresExplicitUltimaPath = false;
            if (string.IsNullOrEmpty(Files.RootDir))
            {
                InfusionProxy.Console.Info("Cannot find Ultima Online installation.");
                requiresExplicitUltimaPath = true;
            }

            if (!string.IsNullOrEmpty(Files.RootDir) && !Directory.Exists(Files.RootDir))
            {
                InfusionProxy.Console.Info($"Ultima Online installation path {Files.RootDir} doesn't exsist.");
                requiresExplicitUltimaPath = true;
            }

            if (requiresExplicitUltimaPath)
            {
                var clientDirectory = Path.GetDirectoryName(options.ClientExePath);
                InfusionProxy.Console.Info($"Client path is {options.ClientExePath}. Assuming Ultima Online files are in {clientDirectory}.");
                Files.SetMulPath(clientDirectory);
            }

            InfusionProxy.Console.Debug($"Loading mul files from {Files.RootDir}.");
        }

        private static ushort GetProxyPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            return (ushort)port;
        }

    }
}
