using Avalonia.Diagnostics.ViewModels;
using Infusion.LegacyApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.Main
{
    public sealed class MainViewModel : ViewModelBase
    {
        private InjectionConfiguration configuration;
        private IMainServices mainServices;

        public void SetServices(InjectionConfiguration configuration, IMainServices mainServices)
        {
            this.configuration = configuration;
            this.mainServices = mainServices;

            RaisePropertyChanged(nameof(AlwaysOnTop));
        }

        public bool Light
        {
            get => configuration?.Options.Light ?? false;
            set
            {
                if (configuration != null)
                {
                    configuration.Options.Light = value;
                    this.mainServices?.SetLightFiltering(value);
                }
                this.RaisePropertyChanged();
            }
        }

        public bool AutoOpenGui
        {
            get => mainServices?.AutoOpen ?? false;
            set
            {
                if (mainServices != null)
                    mainServices.AutoOpen = value;
                this.RaisePropertyChanged();
            }
        }

        public bool AlwaysOnTop
        {
            get => configuration?.Window.AlwaysOnTop ?? false;
            set
            {
                if (configuration != null)
                    configuration.Window.AlwaysOnTop = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
