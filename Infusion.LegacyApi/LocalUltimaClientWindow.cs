using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Infusion.LegacyApi
{
    internal class LocalUltimaClientWindow : IUltimaClientWindow
    {
        private readonly Process ultimaClientProcess;

        public LocalUltimaClientWindow(Process ultimaClientProcess)
        {
            this.ultimaClientProcess = ultimaClientProcess;
        }

        public void SetTitle(string title)
        {
            SetWindowText(ultimaClientProcess.MainWindowHandle, title);
        }

        [DllImport("user32.dll")]
        static extern int SetWindowText(IntPtr hWnd, string text);
    }
}