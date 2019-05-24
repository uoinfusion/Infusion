using Avalonia.Data.Converters;
using Avalonia.Diagnostics.ViewModels;
using Infusion.Desktop.Profiles;
using Infusion.Launcher.Avalonia.Profile.Classic;
using Infusion.Launcher.Avalonia.Profile.Cross;
using Infusion.Launcher.Avalonia.Profile.Orion;
using Infusion.Proxy.Launcher;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infusion.Launcher.Avalonia.Profile
{
    public sealed class ProfileViewModel : ReactiveObject
    {
        private readonly LauncherOptions launcherOptions;

        public LaunchProfile Profile { get; }

        public static ProtocolVersion[] ProtocolVersions { get; } = new[]
        {
            new ProtocolVersion() { Version = new Version(3, 0, 0), Label = ">= 3.0.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 0, 0), Label = ">= 7.0.0.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 9, 0), Label = ">= 7.0.9.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 16, 0), Label = ">= 7.0.16.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 18, 0), Label = ">= 7.0.18.0" },
            new ProtocolVersion() { Version = new Version(7, 0, 33, 0), Label = ">= 7.0.33.0" },
        };

        public ProfileViewModel(LaunchProfile profile, LauncherOptions launcherOptions)
        {
            Profile = profile;
            this.launcherOptions = launcherOptions;
            CrossOptions = new CrossOptionsViewModel(this.launcherOptions.Cross);
            OrionOptions = new OrionOptionsViewModel(this.launcherOptions.Orion);
            ClassicOptions = new ClassicOptionsViewModel(this.launcherOptions.Classic);
        }

        public string Name
        {
            get => Profile.Name;
            set
            {
                Profile.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public string ServerAddress
        {
            get => launcherOptions.ServerEndpoint ?? string.Empty;
            set
            {
                launcherOptions.ServerEndpoint = value;
                this.RaisePropertyChanged();
            }
        }

        public string UserName
        {
            get => launcherOptions.UserName ?? string.Empty;
            set
            {
                launcherOptions.UserName = value;
                this.RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => launcherOptions.Password ?? string.Empty;
            set
            {
                launcherOptions.Password = value;
                this.RaisePropertyChanged();
            }
        }

        public string Id => Profile.Id;

        public UltimaClientType ClientType
        {
            get => launcherOptions.ClientType;
            set
            {
                launcherOptions.ClientType = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged(nameof(CrossOptionsVisible));
                this.RaisePropertyChanged(nameof(OrionOptionsVisible));
                this.RaisePropertyChanged(nameof(ClassicOptionsVisible));
            }
        }

        public string PasswordChar => showPassword ? null : "●";

        private bool showPassword;
        public bool ShowPassword
        {
            get => showPassword;
            set
            {
                showPassword = value;
                this.RaisePropertyChanged();
                this.RaisePropertyChanged("PasswordChar");
            }
        }

        public string InitialScript
        {
            get => launcherOptions.InitialScriptFileName;
            set
            {
                launcherOptions.InitialScriptFileName = value;
                this.RaisePropertyChanged();
            }
        }

        public ProtocolVersion SelectedProtocolVersion
        {
            get => ProtocolVersions.FirstOrDefault(x => x.Version == launcherOptions.ProtocolVersion) ?? ProtocolVersions.First();
            set
            {
                launcherOptions.ProtocolVersion = value.Version;
                this.RaisePropertyChanged();
            }
        }

        public CrossOptionsViewModel CrossOptions { get; }
        public bool CrossOptionsVisible => ClientType == UltimaClientType.CrossUO;

        public OrionOptionsViewModel OrionOptions { get; }
        public bool OrionOptionsVisible => ClientType == UltimaClientType.Orion;

        public ClassicOptionsViewModel ClassicOptions { get; }
        public bool ClassicOptionsVisible => ClientType == UltimaClientType.Classic;
    }
}
