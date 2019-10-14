using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.Launcher.Generic
{
    public sealed class GenericLauncherOptions
    {
        public string DataPath { get; set; }
        public ushort Port { get; set; }
        public EncryptionSetup Encryption { get; set; }
        public Version EncryptionVersion { get; set; }

        internal bool Validate(out string validationMessage)
        {
            validationMessage = string.Empty;
            return true;
        }
    }
}
