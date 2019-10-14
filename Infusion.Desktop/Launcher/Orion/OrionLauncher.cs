using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.Orion
{
    public class OrionLauncher : ILauncher
    {
        public Task Launch(IConsole console, InfusionProxy proxy, LauncherOptions options)
        {
            var proxyPort = options.GetDefaultProxyPort();
            var proxyTask = proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = options.ResolveServerEndpoint().Result,
                LocalProxyPort = proxyPort,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = EncryptionSetup.Autodetect,
                LoginEncryptionKey = null,
            });

            var ultimaExecutableInfo = new FileInfo(options.Orion.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");

                return proxyTask;
            }
            var ultimaExecutablePath = ultimaExecutableInfo.FullName;

            var account = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.UserName)).Replace("-", "");
            var password = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.Password)).Replace("-", "");

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = ultimaExecutableInfo.DirectoryName;

            var insensitiveArguments = $"-autologin:0 -savepassword:0 \"-login 127.0.0.1,{proxyPort}\"";
            var sensitiveArguments = $" -account:{account},{password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + $" -account:{account},<password censored>";

            console.Info($"Staring {ultimaExecutablePath} {argumentsInfo}");

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                console.Error($"Cannot start {ultimaExecutablePath}.");
                return proxyTask;
            }

            ClientProcessWatcher.Watch(ultimaClientProcess);
            proxy.SetClientWindowHandle(ultimaClientProcess);

            return proxyTask;
        }
    }
}
