using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Proxy.Launcher.Orion
{
    public class OrionLauncherOptions : ICloneable
    {
        public string ClientExePath { get; set; }

        object ICloneable.Clone() => Clone();
        public OrionLauncherOptions Clone()
        {
            var newOrion = new OrionLauncherOptions();
            newOrion.ClientExePath = ClientExePath;

            return newOrion;
        }

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
