using Avalonia.Diagnostics.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.Main
{
    public sealed class MainViewModel : ViewModelBase
    {
        private readonly InjectionConfiguration configuration;

        public MainViewModel(InjectionConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public bool Light
        {
            get => configuration.Options.Light;
            set
            {
                configuration.Options.Light = value;
                this.RaisePropertyChanged();
            }
        }

        public bool AutoOpenGui
        {
            get => configuration.Window.AutoOpen;
            set
            {
                configuration.Window.AutoOpen = value;
                this.RaisePropertyChanged();
            }
        }

        public bool AlwaysOnTop
        {
            get => configuration.Window.AlwaysOnTop;
            set
            {
                configuration.Window.AlwaysOnTop = value;
                this.RaisePropertyChanged();
            }
        }
    }
}
