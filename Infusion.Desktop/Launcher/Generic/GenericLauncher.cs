using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO.Encryption.Login;
using Infusion.LegacyApi.Console;
using Infusion.Proxy;

namespace Infusion.Desktop.Launcher.Generic
{
    internal sealed class GenericLauncher : ILauncher
    {
        public Task Launch(IConsole console, InfusionProxy proxy, LauncherOptions options)
        {
            var task = proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = options.ServerEndpoint,
                ServerEndPoint = options.ResolveServerEndpoint().Result,
                LocalProxyPort = options.Generic.Port,
                ProtocolVersion = options.ProtocolVersion,
                Encryption = options.Generic.Encryption,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(options.Generic.EncryptionVersion)
            });

            return task;
        }
    }
}
