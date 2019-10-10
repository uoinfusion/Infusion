using Infusion.IO.Encryption.Login;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Ultima;

namespace Infusion.Desktop.Launcher.Official
{
    public sealed class OfficialClientLauncherOptions
    {

        private string clientExePath;
        public string ClientExePath
        {
            get
            {
                if (string.IsNullOrEmpty(clientExePath))
                {
                    if (!string.IsNullOrEmpty(Files.RootDir))
                        clientExePath = Path.Combine(Files.RootDir, "client.exe");
                    else
                        clientExePath = "client.exe";
                }

                return clientExePath;
            }

            set
            {
                clientExePath = value;
            }
        }

        public EncryptionSetup Encryption { get; set; } = EncryptionSetup.Autodetect;
        public Version EncryptionVersion { get; set; }

        internal bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ClientExePath))
            {
                validationMessage = "Path to ClassicUO client exe not set.";

                return false;
            }

            if (Encryption == EncryptionSetup.EncryptedServer && EncryptionVersion == null)
            {
                validationMessage = "Please, set encryption version.";
                return false;
            }

            validationMessage = string.Empty;
            return true;

        }
    }
}
