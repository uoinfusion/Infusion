using Infusion.Proxy;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Infusion.Desktop.Launcher
{
    public static class OrionLauncher
    {
        public static void Launch(LauncherOptions options, ushort proxyPort)
        {
            var ultimaExecutablePath = options.Orion.ClientExePath;
            if (!File.Exists(ultimaExecutablePath))
            {
                Program.Console.Error($"File {ultimaExecutablePath} doesn't exist.");
                Program.Console.Info(
                    "Infusion requires that you use a client without encryption. If your Ultima Online server allows using Third Dawn (3.x) clients, " +
                    "you can download a client without encryption: https://ulozto.cz/!9w2rZmJfmcvA/client306m-patches-zip. \n\n" +
                    @"The zip file contains NoCryptClient.exe. Copy it to your Ultima Online installation folder (typically c:\Program Files\Ultima Online 2D)." +
                    "\n\n" +
                    "You can read more about how to setup Infusion properly: https://github.com/uoinfusion/Infusion/wiki/Getting-started." + "\n");

                return;
            }

            Program.Console.Info($"Staring {ultimaExecutablePath}");

            var account = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.UserName)).Replace("-", "");
            var password = BitConverter.ToString(System.Text.Encoding.UTF8.GetBytes(options.Password)).Replace("-", "");

            var info = new ProcessStartInfo(ultimaExecutablePath);
            info.WorkingDirectory = Path.GetDirectoryName(ultimaExecutablePath);
            info.Arguments = $"-autologin:0 -savepassword:0 \"-login 127.0.0.1,{proxyPort}\" -account:{account},{password}";

            var ultimaClientProcess = Process.Start(info);
            if (ultimaClientProcess == null)
            {
                Program.Console.Error($"Cannot start {ultimaExecutablePath}.");
                return;
            }

            Program.SetClientWindowHandle(ultimaClientProcess);

        }
    }
}
