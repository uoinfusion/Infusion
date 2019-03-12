using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Scripts.UOErebor.Extensions.Uoam
{
    internal class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
            public uint lPrivate;
        }

#pragma warning disable S101 // Types should be named in PascalCase
        public struct WNDCLASS
#pragma warning restore S101 // Types should be named in PascalCase
        {
            public uint style;
            public IntPtr lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszClassName;
        }


        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        public delegate IntPtr WndProcDelegate(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32", SetLastError = true)]
        public static extern uint GlobalGetAtomName(ushort nAtom, [MarshalAs(UnmanagedType.LPStr)] StringBuilder lpBuffer, int nSize);

        [DllImport("kernel32", SetLastError = true)]
        public static extern ushort GlobalAddAtom([MarshalAs(UnmanagedType.LPStr)] string lpString);

        [DllImport("kernel32", SetLastError = true)]
        public static extern ushort GlobalDeleteAtom(ushort nAtom);

        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            [MarshalAs(UnmanagedType.LPStr)] string lpClassName,
            [MarshalAs(UnmanagedType.LPStr)] string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
        );

        [DllImport("user32", SetLastError = true)]
        public static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32")]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32", SetLastError = true)]
        public static extern ushort RegisterClass(ref WNDCLASS lpWndClass);

        [DllImport("user32", SetLastError = true)]
        public static extern bool UnregisterClass([MarshalAs(UnmanagedType.LPStr)] string lpClassName, IntPtr hInstance);

        [DllImport("user32")]
        public static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

        [DllImport("user32", SetLastError = true)]
        public static extern int GetMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32")]
        public static extern bool TranslateMessage(ref NativeMessage lpMsg);

        [DllImport("user32")]
        public static extern bool DispatchMessage(ref NativeMessage lpMsg);

        [DllImport("user32")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern IntPtr FindWindow([MarshalAs(UnmanagedType.LPStr)] string lpClassName, [MarshalAs(UnmanagedType.LPStr)] string lpWindowName);

        [DllImport("user32")]
        public static extern bool IsWindow(IntPtr hWnd);
    }

}
