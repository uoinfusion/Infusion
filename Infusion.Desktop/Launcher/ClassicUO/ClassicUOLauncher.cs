using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.ClassicUO
{
    public class ClassicUOLauncher : ILauncher
    {
        public Task StartProxy(InfusionProxy proxy, LauncherOptions options, IPEndPoint serverEndPoint, ushort proxyPort)
        {
            return proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = serverEndPoint,
                LocalProxyPort = proxyPort,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = options.ClassicUO.EncryptionSetup,
                LoginEncryptionKey = options.ClassicUO.GetEncryptionKey()
            });
        }

        public void Launch(IConsole console, InfusionProxy proxy, LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutableInfo = new FileInfo(options.ClassicUO.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");
                return;
            }

            var account = options.UserName;
            var password = options.Password;

            var info = new ProcessStartInfo(ultimaExecutableInfo.FullName);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutableInfo.DirectoryName);

            var insensitiveArguments = $"-ip 127.0.0.1 -port {proxyPort} -username {account}";
            var sensitiveArguments = $" -password {password}";
            info.Arguments = insensitiveArguments + sensitiveArguments;

            var argumentsInfo = insensitiveArguments + " -password <censored>";

            console.Info($"Staring {ultimaExecutableInfo.FullName} {argumentsInfo}");

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                console.Error($"Cannot start {ultimaExecutableInfo.FullName}.");
                return;
            }

            ClientProcessWatcher.Watch(ultimaClientProcess);
            proxy.SetClientWindowHandle(ultimaClientProcess);
        }
    }
}
