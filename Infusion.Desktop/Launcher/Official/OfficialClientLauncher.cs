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
        public Task Launch(IConsole console, InfusionProxy proxy, LauncherOptions options)
        {
            var proxyPort = options.GetDefaultProxyPort();

            var proxyTask = proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = options.ResolveServerEndpoint().Result,
                LocalProxyPort = proxyPort,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = options.Official.Encryption,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(options.Official.EncryptionVersion)
            });

            var ultimaExecutableInfo = new FileInfo(options.Official.ClientExePath);
            if (!ultimaExecutableInfo.Exists)
            {
                console.Error($"File {ultimaExecutableInfo.FullName} doesn't exist.");
                return proxyTask;
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
                return proxyTask;
            }

            ClientProcessWatcher.Watch(ultimaClientProcess);
            proxy.SetClientWindowHandle(ultimaClientProcess);

            return proxyTask;
        }
    }
}
