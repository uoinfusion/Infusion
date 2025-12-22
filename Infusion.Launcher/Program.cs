using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Infusion.Launcher;

internal static class Program
{
    private static void Main(string[] args)
    {
        var success = TryStart(@"bin\Infusion.Desktop.exe", args) || TryStart(@"infusion\bin\Infusion.Desktop.exe", args);
        if (!success)
        {
            MessageBox.Show(@"Cannot find bin\Infusion.Desktop.exe file or infusion\bin\Infusion.Desktop.", @"Infusion");
        }
    }

    private static bool TryStart(string infusionExe, string[] args)
    {
        if (!File.Exists(infusionExe))
        {
            return false;
        }

        var startInfo = new ProcessStartInfo(infusionExe);
        if (args.Any(x => x.Trim().Equals("--elevated", StringComparison.OrdinalIgnoreCase)))
        {
            startInfo.Verb = "runas";
        }

        Process.Start(startInfo);
        return true;
    }
}