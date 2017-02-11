using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UltimaRX.Nazghul.Proxy;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

namespace Infusion.Desktop
{
    public partial class InfusionWindow
    {
        private readonly ConsoleContent consoleContent = new ConsoleContent();

        public InfusionWindow()
        {
            InitializeComponent();

            Program.Console = new MultiplexLogger(Program.Console, new InfusionConsoleLogger(consoleContent, this.Dispatcher));
            DataContext = consoleContent;

            var notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Icon = System.Drawing.SystemIcons.WinLogo;
            notifyIcon.Visible = true;
            notifyIcon.DoubleClick += (sender, args) =>
            {
                this.Show();
                this.WindowState = WindowState.Normal;
            };

            _inputBlock.Focus();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                this.Hide();

            base.OnStateChanged(e);
        }

        private void _inputBlock_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                RunCommand();
                _inputBlock.Focus();
                _scroller.ScrollToBottom();
            }
        }

        private void RunCommand()
        {
            string text = _inputBlock.Text;

            if (Injection.CommandHandler.IsInvocationSyntax(text))
                Injection.CommandHandler.Invoke(text);
            else
                Injection.Say(text);

            _inputBlock.Text = string.Empty;
            _inputBlock.Focus();
            _scroller.ScrollToBottom();
        }
    }
}
