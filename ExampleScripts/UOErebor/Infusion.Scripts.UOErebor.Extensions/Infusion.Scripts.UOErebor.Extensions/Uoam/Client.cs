using Infusion.LegacyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Infusion.Scripts.UOErebor.Extensions.Uoam.Native;

namespace Infusion.Scripts.UOErebor.Extensions.Uoam
{
    public static class Client
    {
        private static readonly object startLock = new object();
        private static bool started;
        private static CancellationTokenSource cancellation;
        private const string ApiKey = "UOASSIST-TP-MSG-WND";
        private static int commandOffset;

        public static ScriptTrace Trace { get; } = UO.Trace.Create();
        public static Action<string, ObjectId, Color> OnMessage { get; set; }

        public static void Start()
        {
            lock (startLock)
            {
                if (started)
                    return;

                cancellation = new CancellationTokenSource();
                var thread = new Thread(new ThreadStart(Run));
                thread.IsBackground = true;
                thread.Start();

                started = true;
            }
        }

        public static void Stop()
        {
            cancellation.Cancel();
        }

        private static string ReadAtomAndDelete(ushort atom)
        {
            var result = new StringBuilder(255);

            if (GlobalGetAtomName(atom, result, result.Capacity) == 0)
            {
                Trace.Log("GlobalGetAtomName error");
                return string.Empty;
            }

            GlobalDeleteAtom(atom);

            return result.ToString();
        }

        private static IntPtr WndProc(IntPtr hwnd, uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            switch (uMsg)
            {

                case 0x400 + 200:
                    return new IntPtr(1);

                case 0x400 + 202:
                    return new IntPtr(UO.Me.Location.X | (UO.Me.Location.Y << 16));

                case 0x400 + 207:
                    var messageColor = (Color)(ushort)wParam.ToInt32();
                    var message = ReadAtomAndDelete((ushort)lParam);

                    Trace.Log($"color: {messageColor}, id: {wParam.ToInt32()}, message: {message}");
                    Task.Run(() => {
                        try
                        {
                            OnMessage?.Invoke(message, (uint)wParam.ToInt32(), messageColor);
                        }
                        catch (Exception ex)
                        {
                            UO.Log(ex.ToString());
                        }
                    });
                    break;

                case 0x400 + 209:
                    ReadAtomAndDelete((ushort)lParam);
                    if (wParam == IntPtr.Zero)
                        return IntPtr.Zero;

                    commandOffset++;
                    return new IntPtr(0x400 + 0x400 + commandOffset);

                default:
                    if (uMsg > 0x400)
                        Trace.Log(string.Format("Unknown message 0x{0:x4} w: 0x{1:x4} l: 0x{2:x4}", uMsg, wParam, lParam));
                    break;

            }

            return DefWindowProc(hwnd, uMsg, wParam, lParam);
        }


        private static void Run()
        {
            PeekMessage(out var message, IntPtr.Zero, 0, 0, 0);

            var wndProc = new WndProcDelegate(WndProc);

            var windowClass = new WNDCLASS();
            windowClass.lpszClassName = ApiKey;
            windowClass.lpfnWndProc = Marshal.GetFunctionPointerForDelegate(wndProc);

            Trace.Log("Registering window");
            var windowClassAtom = RegisterClass(ref windowClass);
            if (windowClassAtom == 0 && Marshal.GetLastWin32Error() != 1410)
            {
                Trace.Log("failed");
                return;
            }

            Trace.Log("Creating window");
            var windowHandle = CreateWindowEx(0, ApiKey, ApiKey, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            if (windowHandle == IntPtr.Zero)
            {
                Trace.Log("failed");
                return;
            }

            Trace.Log("Entering main loop");

            try
            {
                while (true)
                {
                    while (PeekMessage(out message, IntPtr.Zero, 0, 0, 0))
                    {
                        if (GetMessage(out message, IntPtr.Zero, 0, 0) == -1)
                        {
                            Trace.Log("GetMessage failed");
                            return;
                        }

                        TranslateMessage(ref message);
                        DispatchMessage(ref message);
                    }
                    UO.CheckCancellation();
                    Thread.Sleep(100);
                }

            }
            catch (Exception ex)
            {
                Trace.Log(ex.ToString());
            }
            finally
            {
                Trace.Log("Destroying window");
                if (!DestroyWindow(windowHandle))
                    Trace.Log("Cannot destroy");

                Trace.Log("Unregistering window");
                if (!UnregisterClass(ApiKey, IntPtr.Zero))
                    Trace.Log("Cannot unregister");

                lock (startLock)
                    started = false;
            }
        }
    }
}
