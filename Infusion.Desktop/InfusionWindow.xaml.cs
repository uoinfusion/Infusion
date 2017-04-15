using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Infusion.Desktop.Launcher;
using Infusion.Desktop.Profiles;
using Infusion.Proxy.LegacyApi;
using Application = System.Windows.Application;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private NotifyIcon notifyIcon;

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

            Legacy.CommandHandler.RegisterCommand(new Command("reload", () => Dispatcher.Invoke(() => _scriptsControl.Reload())));
            Legacy.CommandHandler.RegisterCommand(new Command("edit", () => Dispatcher.Invoke(() => _scriptsControl.Edit())));
        }

        public void Initialize(LauncherOptions options)
        {
            _console.Initialize(options);
            _scriptsControl.Initialize(_console.ScriptEngine, options);
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
            if (WindowState == WindowState.Minimized && ProfileRepositiory.SelectedProfile.Options.HideWhenMinimized)
                Hide();

            base.OnStateChanged(e);
        }
    }
}