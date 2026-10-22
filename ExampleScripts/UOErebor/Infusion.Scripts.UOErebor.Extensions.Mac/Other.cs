// Cross-platform versions of the UOAM client (Windows messages) and DirectionPad (WPF window) - same API as on Windows.
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi;

namespace Infusion.Scripts.UOErebor.Extensions.Uoam
{
    /// <summary>
    /// Instead of the Windows UOAssist window this listens on 127.0.0.1:47317. uoam-bridge.exe running
    /// under Wine next to uoam.exe connects to it and creates the UOAssist window UOAM talks to
    /// (see Infusion.Mac/tools/uoam-bridge).
    /// </summary>
    public static class Client
    {
        public const int Port = 47317;
        private static readonly object startLock = new object();
        private static bool started;
        private static CancellationTokenSource cancellation;
        private static TcpClient current;
        private static StreamWriter writer;
        private static readonly object writeLock = new object();

        public static ScriptTrace Trace { get; } = UO.Trace.Create();
        public static Action<string, ObjectId, Color> OnMessage { get; set; }
        public static bool IsBridgeConnected => current?.Connected == true;

        public static void Start()
        {
            lock (startLock)
            {
                if (started)
                    return;
                cancellation = new CancellationTokenSource();
                var thread = new Thread(() => Run(cancellation.Token)) { IsBackground = true, Name = "UOAM bridge" };
                thread.Start();
                started = true;
            }
        }

        public static void Stop()
        {
            cancellation?.Cancel();
        }

        /// <summary>Replacement of FindWindowA("CUOAMWindow") in scripts - non-zero when the bridge is connected.</summary>
        public static IntPtr MacFindUoam() => IsBridgeConnected ? new IntPtr(1) : IntPtr.Zero;
        public static IntPtr MacFindUoam(string className, string windowName) => MacFindUoam();

        /// <summary>Replacement of SendMessage(hwnd, msg, w, l) to the UOAM window - sent through the bridge.</summary>
        public static int MacSendMessage(IntPtr hwnd, int msg, int wParam, int lParam)
        {
            Send($"S {msg} {wParam} {lParam}");
            return 0;
        }

        private static void Send(string line)
        {
            lock (writeLock)
            {
                try { writer?.WriteLine(line); }
                catch { }
            }
        }

        private static void Run(CancellationToken token)
        {
            TcpListener listener;
            try
            {
                listener = new TcpListener(IPAddress.Loopback, Port);
                listener.Start();
            }
            catch (Exception ex)
            {
                Trace.Log($"UOAM bridge: cannot listen on port {Port}: {ex.Message}");
                lock (startLock) started = false;
                return;
            }

            Trace.Log($"UOAM bridge: waiting for uoam-bridge on 127.0.0.1:{Port}");
            using (token.Register(() => { try { listener.Stop(); } catch { } }))
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client;
                    try { client = listener.AcceptTcpClient(); }
                    catch { break; }

                    current?.Close();
                    current = client;
                    var stream = client.GetStream();
                    lock (writeLock)
                        writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true, NewLine = "\n" };

                    UO.Log("UOAM connected.");
                    var reader = new Thread(() => ReadLoop(client, stream)) { IsBackground = true, Name = "UOAM reader" };
                    reader.Start();

                    // player position to UOAM
                    while (!token.IsCancellationRequested && client.Connected)
                    {
                        try
                        {
                            if (UO.IsLoginConfirmed && UO.Me != null)
                                Send($"P {UO.Me.Location.X} {UO.Me.Location.Y}");
                        }
                        catch { }
                        Thread.Sleep(250);
                        if (!reader.IsAlive) break;
                    }

                    try { client.Close(); } catch { }
                    UO.Log("UOAM disconnected.");
                }
            }

            lock (startLock) started = false;
        }

        private static void ReadLoop(TcpClient client, NetworkStream stream)
        {
            try
            {
                using var r = new StreamReader(stream, Encoding.UTF8);
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    if (line.StartsWith("T "))
                    {
                        var rest = line.Substring(2);
                        var space = rest.IndexOf(' ');
                        var w = space > 0 ? rest.Substring(0, space) : rest;
                        var text = space > 0 ? rest.Substring(space + 1) : string.Empty;
                        if (!uint.TryParse(w, out var wParam)) wParam = 0;
                        Trace.Log($"color: {wParam}, message: {text}");
                        Task.Run(() =>
                        {
                            try { OnMessage?.Invoke(text, wParam, (Color)(ushort)wParam); }
                            catch (Exception ex) { UO.Log(ex.ToString()); }
                        });
                    }
                }
            }
            catch { }
            finally
            {
                try { client.Close(); } catch { }
            }
        }
    }
}

namespace Infusion.Scripts.UOErebor.Extensions
{
    public static class DirectionPad
    {
        public static void Show() => UO.Log("DirectionPad is not available on this platform.");
        public static void Hide() { }
    }
}
