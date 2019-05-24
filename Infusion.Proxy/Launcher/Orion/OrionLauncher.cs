using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Infusion.Proxy.Launcher.Orion
{
    public static class OrionLauncher
    {
        public static void Launch(LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutablePath = options.Orion.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                InfusionProxy.Console.Error($"File {ultimaExecutablePath} doesn't exist.");

                return;
            }

            var account = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.UserName)).Replace("-", "");
            var password = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.Password)).Replace("-", "");

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutablePath);

            var insensitiveArguments = $"-autologin:0 -savepassword:0 \"-login 127.0.0.1,{proxyPort}\"";
            var sensitiveArguments = $" -account:{account},{password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + $" -account:{account},<password censored>";

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
