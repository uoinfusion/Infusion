using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infusion.Desktop.Profiles;

namespace Infusion.Launcher.Avalonia
{
    public class NullLauncher : ILauncher
    {
        public Task Launch(LaunchProfile profile) { return Task.FromResult(false); }
    }
}
