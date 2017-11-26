using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infusion.Launcher
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var infusionExe = @"bin\Infusion.Desktop.exe";

            if (!Directory.Exists(@"bin"))
            {
                MessageBox.Show(@"Cannot find bin folder.", @"Infusion");
                return;
            }

            if (!File.Exists(infusionExe))
            {
                MessageBox.Show(@"Cannot find bin\Infusion.Desktop.exe file.", @"Infusion");
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