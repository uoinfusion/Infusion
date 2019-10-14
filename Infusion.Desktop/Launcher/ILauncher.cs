using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher
{
    internal interface ILauncher
    {
        Task Launch(IConsole console, InfusionProxy proxy, LauncherOptions options);
    }
}
