using Infusion.Desktop.Profiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Launcher.Avalonia
{
    public interface ILauncher
    {
        void Launch(LaunchProfile profile);
    }
}
