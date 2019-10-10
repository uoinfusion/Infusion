using Infusion.IO.Encryption.Login;
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

namespace Infusion.Desktop.Launcher.Official
{
    public class OfficialClientLauncher : ILauncher
    {
        public Task StartProxy(InfusionProxy proxy, LauncherOptions options, IPEndPoint serverEndPoint, ushort proxyPort)
        {
            return proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = serverEndPoint,
                LocalProxyPort = proxyPort,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = options.Official.Encryption,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(options.Official.EncryptionVersion)
            });
        }

        public void Launch(IConsole console, InfusionProxy proxy, LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutableInfo = new FileInfo(options.Official.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");
                return;
            }

            var workingDirectory = ultimaExecutableInfo.DirectoryName;

            var loginConfiguration = new LoginConfiguration(workingDirectory);
            console.Info($"Configuring server address: {loginConfiguration.ConfigFile}");
            loginConfiguration.SetServerAddress("127.0.0.1", proxyPort);

            var ultimaConfiguration = new UltimaConfiguration(workingDirectory);
            console.Info($"Configuring user name and password: {ultimaConfiguration.ConfigFile}");
            if (!string.IsNullOrEmpty(options.UserName))
                ultimaConfiguration.SetUserName(options.UserName);
            if (!string.IsNullOrEmpty(options.Password))
                ultimaConfiguration.SetPassword(options.EncryptPassword());

            console.Info($"Staring {ultimaExecutableInfo.FullName} from {workingDirectory}");

            var info = new ProcessStartInfo(ultimaExecutableInfo.FullName)
            {
                WorkingDirectory = workingDirectory
            };

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
