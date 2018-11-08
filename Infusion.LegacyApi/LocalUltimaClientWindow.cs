using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Infusion.LegacyApi
{
    internal class LocalUltimaClientWindow : IUltimaClientWindow
    {
        private readonly Process ultimaClientProcess;

        public IntPtr Handle => ultimaClientProcess.MainWindowHandle;

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

        public void PressKey(char ch)
        {
            SendChar(ultimaClientProcess.MainWindowHandle, (int) ch);
        }

        public void PressKey(KeyCode keyCode)
        {
            PostMessage(ultimaClientProcess.MainWindowHandle, WM_KEYDOWN, (int)keyCode, 1);
            PostMessage(ultimaClientProcess.MainWindowHandle, WM_KEYUP, (int)keyCode, 1);
        }

        private static void SendChar(IntPtr hWnd, int value)
        {
            int lParam = 1 | ((OemKeyScan(value) & 0xFF) << 16) | (0x3 << 30);

            PostMessage(hWnd, WM_CHAR, value, lParam);
        }

        public void SendText(string text)
        {
            for (int i = 0; i < text.Length; ++i)
                SendChar(ultimaClientProcess.MainWindowHandle, text[i]);

            SendChar(ultimaClientProcess.MainWindowHandle, '\r');
            SendChar(ultimaClientProcess.MainWindowHandle, '\n');
        }

        private const int WM_CHAR = 0x102;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x0101;

        [DllImport("user32.dll")]
        private static extern int SetWindowText(IntPtr hWnd, string text);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetClientRect(IntPtr hWnd, out WindowBounds lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll")]
        public static extern void SwitchToThisWindow(IntPtr hWnd);

        [DllImport("User32")]
        public static extern bool PostMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        [DllImport("User32")]
        public static extern int OemKeyScan(int wOemChar);

    }
}