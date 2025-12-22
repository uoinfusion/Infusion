using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Utilities
{
    public static class VersionHelpers
    {
        private static Version fileVersion;

        public static Version ProductVersion
        {
            get
            {
                if (fileVersion == null)
                {
                    string versionText = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
                        .FileVersion;
                    if (!Version.TryParse(versionText, out fileVersion))
                        fileVersion = new Version(0, 0, 0, 0);
                }

                return fileVersion;
            }
        } 
    }
}
