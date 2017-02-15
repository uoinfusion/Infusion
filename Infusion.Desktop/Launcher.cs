using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Ultima;
using UltimaRX.Proxy;

namespace Infusion.Desktop
{
    public static class Launcher
    {
        public static async Task Launch(LauncherOptions options)
        {
            var serverEndPoint = await options.ResolveServerEndpoint();
            var proxyTask = Program.Start(serverEndPoint, options.ProxyPort);

            LoginConfiguration.SetServerAddress("127.0.0.1", options.ProxyPort);
            if (!string.IsNullOrEmpty(options.UserName))
                UltimaConfiguration.SetUserName(options.UserName);
            if (!string.IsNullOrEmpty(options.Password))
                UltimaConfiguration.SetPassword(options.EncryptPassword());

            string ultimaExecutablePath = Path.Combine(Files.RootDir, "NoCryptClient.exe");

            Process.Start(ultimaExecutablePath);
        }
    }
}
