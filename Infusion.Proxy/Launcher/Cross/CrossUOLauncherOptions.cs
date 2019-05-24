using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Infusion.Proxy.Launcher.Cross
{
    public class CrossUOLauncherOptions : ICloneable
    {
        public string ClientExePath { get; set; }

        object ICloneable.Clone() => Clone();

        public CrossUOLauncherOptions Clone()
        {
            var newCross = new CrossUOLauncherOptions();

            newCross.ClientExePath = ClientExePath;

            return newCross;
        }

        internal bool Validate(out string validationMessage)
        {
            if (string.IsNullOrEmpty(ClientExePath))
            {
                validationMessage = "Path to CrossUO client exe not set.";

                return false;
            }

            validationMessage = string.Empty;
            return true;
        }
    }
}
