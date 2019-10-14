using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.CrossUO
{
    public class CrossUOLauncher : ILauncher
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

            var ultimaExecutableInfo = new FileInfo(options.Cross.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");
                return proxyTask;
            }

            var account = options.UserName;
            var password = options.Password;

            var info = new ProcessStartInfo(ultimaExecutableInfo.FullName);
            info.WorkingDirectory = ultimaExecutableInfo.DirectoryName;

            var insensitiveArguments = $"--host 127.0.0.1 --port {proxyPort} --login {account}";
            var sensitiveArguments = $" --password {password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + " --password <censored>";

            console.Info($"Staring {ultimaExecutableInfo.FullName} {argumentsInfo}");

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                console.Error($"Cannot start {ultimaExecutableInfo.FullName}.");
                return proxyTask;
            }

            ClientProcessWatcher.Watch(ultimaClientProcess);
            proxy.SetClientWindowHandle(ultimaClientProcess);

            return proxyTask;
        }
    }
}
