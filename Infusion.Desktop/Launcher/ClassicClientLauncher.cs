using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    public static class ClassicClientLauncher
    {
        public static void Launch(IConsole console, InfusionProxy proxy, LauncherOptions options, ushort proxyPort)
        {
            string ultimaExecutablePath = options.Classic.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                console.Error($"File {ultimaExecutablePath} doesn't exist.");
                return;
            }

            string workingDirectory = Path.GetDirectoryName(ultimaExecutablePath);

            var loginConfiguration = new LoginConfiguration(workingDirectory);
            console.Info($"Configuring server address: {loginConfiguration.ConfigFile}");
            loginConfiguration.SetServerAddress("127.0.0.1", proxyPort);

            var ultimaConfiguration = new UltimaConfiguration(workingDirectory);
            console.Info($"Configuring user name and password: {ultimaConfiguration.ConfigFile}");
            if (!string.IsNullOrEmpty(options.UserName))
                ultimaConfiguration.SetUserName(options.UserName);
            if (!string.IsNullOrEmpty(options.Password))
                ultimaConfiguration.SetPassword(options.EncryptPassword());

            console.Info($"Staring {ultimaExecutablePath} from {workingDirectory}");

            var info = new ProcessStartInfo(ultimaExecutablePath)
            {
                WorkingDirectory = workingDirectory
            };

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                console.Error($"Cannot start {ultimaExecutablePath}.");
                return;
            }

            proxy.SetClientWindowHandle(ultimaClientProcess);
        }
    }
}
