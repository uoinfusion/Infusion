using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Desktop.Launcher.Orion
{
    public class OrionLauncherOptions
    {
        public string ClientExePath { get; set; }

        internal bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ClientExePath))
            {
                validationMessage = "Path to Orion client exe not set.";

                return false;
            }

            validationMessage = string.Empty;
            return true;
        }
    }
}
