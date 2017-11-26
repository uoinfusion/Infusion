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
        private static Version productVersion;

        public static Version ProductVersion
        {
            get
            {
                if (productVersion == null)
                {
                    string versionText = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location)
                        .ProductVersion;
                    if (!Version.TryParse(versionText, out productVersion))
                        productVersion = new Version(0, 0, 0, 0);
                }

                return productVersion;
            }
        } 
    }
}
