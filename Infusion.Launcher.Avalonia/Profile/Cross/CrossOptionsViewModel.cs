using Infusion.Proxy.Launcher.Cross;
using ReactiveUI;

namespace Infusion.Launcher.Avalonia.Profile.Cross
{
    public sealed class CrossOptionsViewModel : ReactiveObject
    {
        private readonly CrossUOLauncherOptions options;

        public CrossOptionsViewModel(CrossUOLauncherOptions options)
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
