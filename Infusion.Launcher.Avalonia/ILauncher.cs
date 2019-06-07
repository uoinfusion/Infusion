using Infusion.Desktop.Profiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Launcher.Avalonia
{
    public interface ILauncher
    {
        Task Launch(LaunchProfile profile);
    }
}
