using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Infusion.Proxy.Launcher.Cross
{
    public static class CrossUOLauncher
    {
        public static void Launch(LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutablePath = options.Cross.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                InfusionProxy.Console.Error($"File {ultimaExecutablePath} doesn't exist.");

                return;
            }

            var account = options.UserName;
            var password = options.Password;

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutablePath);

            var insensitiveArguments = $"--host 127.0.0.1 --port {proxyPort} --login {account}";
            var sensitiveArguments = $" --password {password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + " --password <censored>";

            InfusionProxy.Console.Info($"Staring {ultimaExecutablePath} {argumentsInfo}");

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                InfusionProxy.Console.Error($"Cannot start {ultimaExecutablePath}.");
                return;
            }

            InfusionProxy.SetClientWindowHandle(ultimaClientProcess);
        }
    }
}
