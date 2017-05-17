using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Infusion.Desktop.Launcher;
using Infusion.Desktop.Profiles;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private NotifyIcon notifyIcon;
        private string scriptFileName;

        public InfusionWindow()
        {
            InitializeComponent();

            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = System.Drawing.Icon.FromHandle(new Bitmap(Properties.Resources.infusion).GetHicon());
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += (sender, args) =>
            {
                Show();
                WindowState = WindowState.Normal;
            };

            Legacy.CommandHandler.RegisterCommand(new Command("reload", () => Dispatcher.Invoke(() => Reload())));
            Legacy.CommandHandler.RegisterCommand(new Command("edit", () => Dispatcher.Invoke(() => Edit())));
        }

        public void Initialize(LauncherOptions options)
        {
            _console.Initialize(options);
            this.scriptFileName = options.InitialScriptFileName;
        }

        public void Edit()
        {
            if (!string.IsNullOrEmpty(scriptFileName) && File.Exists(scriptFileName))
            {
                string scriptPath = System.IO.Path.GetDirectoryName(scriptFileName);
                _console.ScriptEngine.ScriptRootPath = scriptPath;

                var roslynPadWindow = new RoslynPad.MainWindow(_console.ScriptEngine, scriptPath);
                roslynPadWindow.Show();
            }
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
                await _console.ScriptEngine.ExecuteScript(scriptFileName);
            }
            else
            {
                // TODO: handle error
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            ProfileRepositiory.SaveProfile(ProfileRepositiory.SelectedProfile);

            if (notifyIcon != null)
            {
                notifyIcon.Dispose();
                notifyIcon = null;
            }

            Application.Current.Shutdown();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized && Program.Configuration.HideWhenMinimized)
                Hide();

            base.OnStateChanged(e);
        }
    }
}