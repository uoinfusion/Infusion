using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Infusion.Desktop.Launcher
{
    public static class ClassicUOLauncher
    {
        public static void Launch(IConsole console, InfusionProxy proxy, LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutablePath = options.ClassicUO.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                console.Error($"File {ultimaExecutablePath} doesn't exist.");
                return;
            }

            var account = options.UserName;
            var password = options.Password;

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutablePath);

            string insensitiveArguments = $"-ip 127.0.0.1 -port {proxyPort} -username {account}";
            string sensitiveArguments = $" -password {password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            string argumentsInfo = insensitiveArguments + " -password <censored>";

            console.Info($"Staring {ultimaExecutablePath} {argumentsInfo}");

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
