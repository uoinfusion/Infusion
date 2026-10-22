// macOS replacement of System.Media.SoundPlayer (System.Windows.Extensions) used by scripts - plays sounds with /usr/bin/afplay.
using System.Diagnostics;
using System.IO;

namespace System.Media
{
    public class SoundPlayer : IDisposable
    {
        private Process current;
        private volatile bool looping;

        public SoundPlayer() { }
        public SoundPlayer(string soundLocation) { SoundLocation = soundLocation; }
        public SoundPlayer(Stream stream) { Stream = stream; }

        public string SoundLocation { get; set; }
        public Stream Stream { get; set; }
        public bool IsLoadCompleted => true;
        public int LoadTimeout { get; set; } = 10000;
        public object Tag { get; set; }

        public void Load() { }
        public void LoadAsync() { }

        public void Play() => System.Threading.Tasks.Task.Run(() => PlaySync());

        public void PlaySync()
        {
            var path = ResolvePath();
            if (path == null)
                return;
            try
            {
                var p = Process.Start(new ProcessStartInfo("/usr/bin/afplay") { ArgumentList = { path }, UseShellExecute = false });
                current = p;
                p?.WaitForExit();
            }
            catch { }
        }

        public void PlayLooping()
        {
            looping = true;
            System.Threading.Tasks.Task.Run(() => { while (looping) { PlaySync(); } });
        }

        public void Stop()
        {
            looping = false;
            try { if (current != null && !current.HasExited) current.Kill(); } catch { }
        }

        private string tempFile;

        private string ResolvePath()
        {
            if (!string.IsNullOrEmpty(SoundLocation))
            {
                var p = SoundLocation.Replace('\\', '/');
                if (!Path.IsPathRooted(p))
                    p = Path.GetFullPath(p);
                if (File.Exists(p)) return p;
                var dir = Path.GetDirectoryName(p);
                if (dir != null && Directory.Exists(dir))
                {
                    foreach (var f in Directory.GetFiles(dir))
                        if (string.Equals(Path.GetFileName(f), Path.GetFileName(p), StringComparison.OrdinalIgnoreCase))
                            return f;
                }
                return null;
            }

            if (Stream != null)
            {
                tempFile ??= Path.Combine(Path.GetTempPath(), "infusion-sound-" + Guid.NewGuid().ToString("N") + ".wav");
                using (var fs = File.Create(tempFile)) { Stream.Position = 0; Stream.CopyTo(fs); }
                return tempFile;
            }

            return null;
        }

        public void Dispose()
        {
            Stop();
            try { if (tempFile != null) File.Delete(tempFile); } catch { }
        }

        public event EventHandler LoadCompleted { add { } remove { } }
        public event EventHandler SoundLocationChanged { add { } remove { } }
        public event EventHandler StreamChanged { add { } remove { } }
    }

    public class SystemSound
    {
        private readonly string file;
        internal SystemSound(string file) { this.file = file; }
        public void Play() => new SoundPlayer(file).Play();
    }

    public static class SystemSounds
    {
        public static SystemSound Asterisk { get; } = new SystemSound("/System/Library/Sounds/Glass.aiff");
        public static SystemSound Beep { get; } = new SystemSound("/System/Library/Sounds/Ping.aiff");
        public static SystemSound Exclamation { get; } = new SystemSound("/System/Library/Sounds/Funk.aiff");
        public static SystemSound Hand { get; } = new SystemSound("/System/Library/Sounds/Basso.aiff");
        public static SystemSound Question { get; } = new SystemSound("/System/Library/Sounds/Purr.aiff");
    }
}
