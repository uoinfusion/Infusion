using System;
using System.IO;
using Ultima;

namespace Infusion.Proxy.Launcher.Classic
{
    public sealed class ClassicClientLauncherOptions : ICloneable
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

        public EncryptionVersion EncryptionVersion { get; set; }

        public ClassicClientLauncherOptions Clone()
        {
            var newClassic = new ClassicClientLauncherOptions();

            newClassic.ClientExePath = ClientExePath;
            newClassic.Encryption = Encryption;
            newClassic.EncryptionVersion = EncryptionVersion;

            return newClassic;
        }

        object ICloneable.Clone() => Clone();
    }
}
