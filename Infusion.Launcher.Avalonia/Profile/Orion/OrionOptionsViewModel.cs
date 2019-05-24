using Infusion.Proxy.Launcher.Orion;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Launcher.Avalonia.Profile.Orion
{
    public sealed class OrionOptionsViewModel : ReactiveObject
    {
        private readonly OrionLauncherOptions options;

        public OrionOptionsViewModel(OrionLauncherOptions options)
        {
            this.options = options;
        }

        public string ClientPath
        {
            get => options.ClientExePath;
            set
            {
                options.ClientExePath = value;
                this.RaisePropertyChanged();
            }
        }

    }
}
