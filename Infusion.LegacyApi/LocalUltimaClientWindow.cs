using System;
using System.Diagnostics;
using System.Drawing;
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

        public WindowBounds? GetBounds()
        {
            if (GetClientRect(ultimaClientProcess.MainWindowHandle, out WindowBounds bounds))
            {
                var topLeft = new Point(bounds.Left, bounds.Top);
                var bottomRight = new Point(bounds.Right, bounds.Bottom);
                if (ClientToScreen(ultimaClientProcess.MainWindowHandle, ref topLeft) && ClientToScreen(ultimaClientProcess.MainWindowHandle, ref bottomRight))
                    return new WindowBounds() { Top = topLeft.Y, Bottom = bottomRight.Y, Left = topLeft.X, Right = bottomRight.X, };
            }

            return null;
        }

        public void Focus()
        {
            SwitchToThisWindow(ultimaClientProcess.MainWindowHandle);
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect(IntPtr hWnd, out WindowBounds lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);
    }
}