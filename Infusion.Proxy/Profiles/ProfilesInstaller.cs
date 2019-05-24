using Infusion.LegacyApi.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Profiles
{
    public class ProfilesInstaller
    {
        private readonly IConsole console;
        private readonly string profilesPath;

        public ProfilesInstaller(IConsole console, string profilesPath)
        {
            this.console = console;
            this.profilesPath = profilesPath;
        }

        public bool Install()
        {
            if (!Directory.Exists(profilesPath) || !Directory.GetFiles(profilesPath).Any())
            {
                console.Info($"Creating folder {profilesPath}");
                try
                {
                    Directory.CreateDirectory(profilesPath);
                }
                catch (Exception ex)
                {
                    console.Error(ex.Message);
                    console.Debug(ex.ToString());

                    return false;
                }
            }

            return true;
        }
    }
}
