using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infusion.Launcher
{
    // This program is here just to start Infusion from distribution root. Users
    // are afraid of bin folder with countless dlls.
    // This means that this project cannot have any 3rd party dependencies - only
    // .NET Framework dependencies are allowed - output has to be single exe file,
    // no dlls or other files.
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var infusionExe = @"bin\Infusion.Console.Wpf.exe";

            if (!Directory.Exists(@"bin"))
            {
                MessageBox.Show(@"Cannot find bin folder.", @"Infusion");
                return;
            }

            if (!File.Exists(infusionExe))
            {
                MessageBox.Show(@"Cannot find bin\Infusion.Console.Wpf.exe file.", @"Infusion");
                return;
            }

            var startInfo = new ProcessStartInfo(infusionExe);
            if (args.Any(x => x.Trim().Equals("--elevated", StringComparison.OrdinalIgnoreCase)))
            {
                startInfo.Verb = "runas";
            }

            Process.Start(startInfo);
        }
    }
}