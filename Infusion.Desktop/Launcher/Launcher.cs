using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Desktop.Launcher.ClassicUO;
using Infusion.Desktop.Launcher.CrossUO;
using Infusion.Desktop.Launcher.Generic;
using Infusion.Desktop.Launcher.Official;
using Infusion.Desktop.Launcher.Orion;
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
                var launcher = GetLauncher(options);
                var serverEndPoint = options.ResolveServerEndpoint().Result;
                ushort proxyPort = options.GetDefaultProxyPort();

                CheckMulFiles(options);

                launcher.Launch(console, proxy, options);

                InterProcessCommunication.StartReceiving();
            });
        }

        private ILauncher GetLauncher(LauncherOptions options)
        {
            switch (options.ClientType)
            {
                case UltimaClientType.Classic:
                    return new OfficialClientLauncher();
                case UltimaClientType.ClassicUO:
                    return new ClassicUOLauncher();
                case UltimaClientType.CrossUO:
                    return new CrossUOLauncher();
                case UltimaClientType.Orion:
                    return new OrionLauncher();
                case UltimaClientType.Generic:
                    return new GenericLauncher();
                default:
                    throw new NotImplementedException(options.ClientType.ToString());
            }
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
    }
}
