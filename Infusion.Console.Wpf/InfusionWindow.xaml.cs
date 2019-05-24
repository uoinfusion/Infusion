using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Infusion.Commands;
using Infusion.Desktop.Console;
using Infusion.Desktop.Profiles;
using Infusion.Desktop.Scripts;
using Infusion.Launcher.Avalonia;
using Infusion.LegacyApi;
using Infusion.Proxy;
using Infusion.Proxy.Launcher;
using Infusion.Proxy.Scripts;
using Infusion.Utilities;
using Application = System.Windows.Application;
using AvaloniaDispatcher = global::Avalonia.Threading.Dispatcher;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private FileConsole fileConsole;
        private InfusionConsole infusionConsole;

        private NotifyIcon notifyIcon;
        private string scriptFileName;
        private LaunchProfile profile;

        internal Lazy<ScriptEngine> ScriptEngine { get; private set; }
        public Lazy<CSharpScriptEngine> CSharpScriptEngine { get; private set; }

        public InfusionWindow()
        {
            InitializeComponent();
            InitializeInfusion();

            infusionConsole.Important($"Infusion {VersionHelpers.ProductVersion}");
            infusionConsole.Info($"Infusion root path {PathUtilities.RootPath}");

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(new Bitmap(Infusion.Console.Wpf.Properties.Resources.infusion).GetHicon());
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += (sender, args) =>
            {
                if (!IsVisible)
                    Show();

                if (WindowState != WindowState.Normal)
                    WindowState = WindowState.Normal;

                Activate();
                Topmost = true;
                Topmost = false;
                Focus();
            };
        }

        private void HandleFileLoggingException(Exception ex)
        {
            infusionConsole.Error($"Error while writing logs to disk. Please, check that Infusion can write to {InfusionProxy.LogConfig.LogPath}.");
            infusionConsole.Important("You can change the log path by setting UO.Configuration.LogPath property or disable packet logging by setting UO.Configuration.LogToFileEnabled = false in your initial script.");
            infusionConsole.Debug(ex.ToString());
        }

        private void InitializeInfusion()
        {
            fileConsole = new FileConsole(InfusionProxy.LogConfig, new CircuitBreaker(HandleFileLoggingException));
            var wpfConsole = _console.CreateWpfConsole();
            infusionConsole = new InfusionConsole(fileConsole, wpfConsole);

            InfusionProxy.Console = infusionConsole;
            var commandHandler = new CommandHandler(InfusionProxy.Console);

            InfusionProxy.Initialize(commandHandler, new SoundPlayer());

            CSharpScriptEngine = new Lazy<CSharpScriptEngine>(() => new CSharpScriptEngine(infusionConsole));
            ScriptEngine = new Lazy<ScriptEngine>(() => new ScriptEngine(CSharpScriptEngine.Value, new InjectionScriptEngine(UO.Injection, infusionConsole)));

            _console.ShowNoDebug();
        }

        private void Cls()
        {
            _console.Clear();
        }

        private void Load(string scriptFileName)
        {
            string name = Path.GetFileName(scriptFileName);
            var scriptPath = Path.GetDirectoryName(scriptFileName);

            if (string.IsNullOrEmpty(scriptPath))
                scriptPath = PathUtilities.GetAbsolutePath("scripts");
            else if (!Path.IsPathRooted(scriptPath))
                scriptPath = Path.Combine(PathUtilities.GetAbsolutePath("scripts"), scriptPath);

            scriptFileName = Path.Combine(scriptPath, name);

            this.scriptFileName = scriptFileName;
            ScriptEngine.Value.ScriptRootPath = scriptPath;

            Reload();
        }

        internal void Initialize(LaunchProfile profile)
        {
            this.profile = profile;
            Title = $"{profile.Name}";

            UO.CommandHandler.RegisterCommand(new Command("reload", () => Dispatcher.Invoke(() => Reload()), false, true,
                "Reloads an initial script file."));
            UO.CommandHandler.RegisterCommand(new Command("edit",
                () => Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action)(() => Edit())),
                false, true, "Opens the script editor."));
            UO.CommandHandler.RegisterCommand(new Command("load", path => Dispatcher.Invoke(() => Load(path)),
                false, true, "Loads a script file."));
            UO.CommandHandler.RegisterCommand(new Command("cls", () => Dispatcher.Invoke(Cls),
                false, true, "Clears console content."));

            UO.CommandHandler.RegisterCommand(new Command("console-show-toggle", () => Dispatcher.Invoke(() => _console.ShowToggle()), false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-speechonly", () => Dispatcher.Invoke(() => _console.ShowSpeechOnly()), false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-game", () => Dispatcher.Invoke(() => _console.ShowGame()), false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-all", () => Dispatcher.Invoke(() => _console.ShowAll()), false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-nodebug", () => Dispatcher.Invoke(() => _console.ShowNoDebug()), false, true));

            var configRepository = new ProfileConfigRepository(profile, this.infusionConsole);
            var launcherOptions = configRepository.Get<LauncherOptions>("launcher.avalonia");
            if (!string.IsNullOrEmpty(launcherOptions.InitialScriptFileName))
                Load(launcherOptions.InitialScriptFileName);

            //if (profile.ConsoleOptions != null)
            //{
            //    this.Left = profile.ConsoleOptions.X;
            //    this.Top = profile.ConsoleOptions.Y;
            //    this.Width = profile.ConsoleOptions.Width;
            //    this.Height = profile.ConsoleOptions.Height;
            //}

            InfusionProxy.LegacyApi.LoginConfirmed += HandleLoginConfirmed;
        }

        private void HandleLoginConfirmed()
        {
            //var logPath = PathUtilities.GetAbsolutePath($"logs\\{Program.LegacyApi.ServerName}\\{profile.LauncherOptions.UserName}\\{Program.LegacyApi.SelectedCharacterName}\\");
            //Program.LogConfig.SetDefaultLogPath(logPath);
        }

        public void Edit()
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                var scriptPath = Path.GetDirectoryName(scriptFileName);

                var roslynPadWindow = new RoslynPad.MainWindow(new RoslynPadScriptEngineBridge(CSharpScriptEngine.Value), scriptPath);
                roslynPadWindow.Show();
            }
            else
                infusionConsole.Error(
                    "Initial script is not set. You can set the initial script by restarting Infusion and setting an absolute path to a script in 'Initial script' edit box at Infusion launcher dialog, or by invoking ,load <absolute path to script>");
        }

        private void Reload()
        {
#pragma warning disable 4014
            Reload(scriptFileName);
#pragma warning restore 4014
        }

        private async Task Reload(string scriptFileName)
        {
            if (string.IsNullOrEmpty(scriptFileName))
            {
                infusionConsole.Error(
    "Initial script is not set. You can set the initial script by restarting Infusion and setting an absolute path to a script in 'Initial script' edit box at Infusion launcher dialog, or by invoking ,load <absolute path to script>");
                return;
            }

            if (!File.Exists(scriptFileName))
            {
                infusionConsole.Error($"Script file doesn't exist: '{scriptFileName}'");
                return;
            }

            InfusionProxy.LegacyApi.Config.Save();
            ProfileRepository.SaveProfile(profile);
            ProfileRepository.FixOptions(profile);

            UO.CommandHandler.BeginTerminate(true);
            UO.CommandHandler.UnregisterAllPublic();

            ScriptEngine.Value.Reset();
            using (var tokenSource = new CancellationTokenSource())
            {
                await ScriptEngine.Value.ExecuteScript(scriptFileName, tokenSource);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ProfileRepository.SelectedProfile != null)
                ProfileRepository.SaveProfile(ProfileRepository.SelectedProfile);

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            fileConsole.Dispose();

            Application.Current.Shutdown();
        }

        private void InfusionWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            AvaloniaDispatcher.UIThread.InvokeAsync(() =>
            {
                var launcherWindow = new Infusion.Launcher.Avalonia.MainWindow(new Launcher(this));

                launcherWindow.Topmost = true;
                launcherWindow.Show();
                launcherWindow.Activate();
                launcherWindow.Focus();
                launcherWindow.Topmost = false;
                //var launcherWindow = new Infusion.Launcher
            });
            //Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action) (() =>
            //{
            //    _console.Initialize();

            //    var launcherWindow = new LauncherWindow(Initialize, infusionConsole);
            //    launcherWindow.Show();
            //    launcherWindow.Activate();
            //}));
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (profile == null)
                return;

            //if (profile.ConsoleOptions == null)
            //    profile.ConsoleOptions = new Console.ConsoleOptions();

            //profile.ConsoleOptions.X = Left;
            //profile.ConsoleOptions.Y = Top;
            //profile.ConsoleOptions.Width = Width;
            //profile.ConsoleOptions.Height = Height;

            InfusionProxy.LegacyApi.Config.Save();
            ProfileRepository.SaveProfile(profile);
        }

        private class Launcher : ILauncher
        {
            private readonly InfusionWindow window;

            public Launcher(InfusionWindow window)
            {
                this.window = window;
            }

            public void Launch(LaunchProfile profile) => window.Initialize(profile);
        }
    }
}