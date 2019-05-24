using System;
using System.Collections.Generic;
using System.Text;
using Infusion.Desktop.Profiles;

namespace Infusion.Launcher.Avalonia
{
    public class NullLauncher : ILauncher
    {
        public void Launch(LaunchProfile profile) { }
    }
}
