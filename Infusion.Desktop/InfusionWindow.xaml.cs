using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;
using Infusion.Commands;
using Infusion.Desktop.Launcher;
using Infusion.Desktop.Profiles;
using Infusion.LegacyApi;
using Infusion.Proxy;
using Infusion.Utilities;
using Application = System.Windows.Application;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private NotifyIcon notifyIcon;
        private string scriptFileName;
        private Profile profile;

        public InfusionWindow()
        {
            InitializeComponent();

            Program.Console.Important($"Infusion {VersionHelpers.ProductVersion}");

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(new Bitmap(Properties.Resources.infusion).GetHicon());
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


            UO.CommandHandler.RegisterCommand(new Command("reload", () => Dispatcher.Invoke(() => Reload()),
                "Reloads an initial script file."));
            UO.CommandHandler.RegisterCommand(new Command("edit",
                () => Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (Action) (() => Edit())),
                "Opens the script editor."));
            UO.CommandHandler.RegisterCommand(new Command("load", path => Dispatcher.Invoke(() => Load(path)),
                "Loads a script file."));
            UO.CommandHandler.RegisterCommand(new Command("cls", () => Dispatcher.Invoke(Cls),
                "Clears console content."));

            UO.CommandHandler.RegisterCommand(new Command("console-show-toggle", () => Dispatcher.Invoke(() => _console.ShowToggle())));
            UO.CommandHandler.RegisterCommand(new Command("console-show-speechonly", () => Dispatcher.Invoke(() => _console.ShowSpeechOnly())));
            UO.CommandHandler.RegisterCommand(new Command("console-show-game", () => Dispatcher.Invoke(() => _console.ShowGame())));
            UO.CommandHandler.RegisterCommand(new Command("console-show-all", () => Dispatcher.Invoke(() => _console.ShowAll())));
        }

        private void Cls()
        {
            _console.Clear();
        }

        private void Load(string scriptFileName)
        {
            this.scriptFileName = scriptFileName;
            var scriptPath = Path.GetDirectoryName(scriptFileName);
            _console.ScriptEngine.ScriptRootPath = scriptPath;

            Reload();
        }

        internal void Initialize(Profile profile)
        {
            this.profile = profile;
            Title = $"{profile.Name}";

            if (!string.IsNullOrEmpty(profile.LauncherOptions.InitialScriptFileName))
                Load(profile.LauncherOptions.InitialScriptFileName);

            if (profile.ConsoleOptions != null)
            {
                this.Left = profile.ConsoleOptions.X;
                this.Top = profile.ConsoleOptions.Y;
                this.Width = profile.ConsoleOptions.Width;
                this.Height = profile.ConsoleOptions.Height;
            }
        }

        public void Edit()
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                var scriptPath = Path.GetDirectoryName(scriptFileName);

                var roslynPadWindow = new RoslynPad.MainWindow(_console.ScriptEngine, scriptPath);
                roslynPadWindow.Show();
            }
            else
                Program.Console.Error(
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
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                UO.CommandHandler.BeginTerminate(true);
                _console.ScriptEngine.Reset();
                using (var tokenSource = new CancellationTokenSource())
                {
                    await _console.ScriptEngine.ExecuteScript(scriptFileName, tokenSource);
                }
            }
            else
                Program.Console.Error(
                    "Initial script is not set. You can set the initial script by restarting Infusion and setting an absolute path to a script in 'Initial script' edit box at Infusion launcher dialog, or by invoking ,load <absolute path to script>");
        }

        protected override void OnClosed(EventArgs e)
        {
            if (ProfileRepositiory.SelectedProfile != null)
                ProfileRepositiory.SaveProfile(ProfileRepositiory.SelectedProfile);

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            _console.Dispose();

            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized && Program.Configuration.HideWhenMinimized)
                Hide();

            base.OnStateChanged(e);
        }

        private void InfusionWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, (Action) (() =>
            {
                _console.Initialize();

                var launcherWindow = new LauncherWindow(Initialize);
                launcherWindow.Show();
                launcherWindow.Activate();
            }));
        }

        private void HandleClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (profile == null)
                return;

            if (profile.ConsoleOptions == null)
                profile.ConsoleOptions = new Console.ConsoleOptions();

            profile.ConsoleOptions.X = Left;
            profile.ConsoleOptions.Y = Top;
            profile.ConsoleOptions.Width = Width;
            profile.ConsoleOptions.Height = Height;

            ProfileRepositiory.SaveProfile(profile);
        }
    }
}