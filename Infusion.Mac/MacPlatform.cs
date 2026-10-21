using System;
using System.Diagnostics;
using System.IO;
using Infusion.LegacyApi;

namespace Infusion.Mac
{
    /// <summary>Plays sounds with macOS afplay (System.Media.SoundPlayer on Windows).</summary>
    internal sealed class MacSoundPlayer : ISoundPlayer
    {
        public void PlayFile(string path)
        {
            try
            {
                path = MacPath.FindCaseInsensitive(Path.GetFullPath(MacPath.Normalize(path)));
                if (!File.Exists(path))
                    return;

                using var p = Process.Start(new ProcessStartInfo("/usr/bin/afplay", $"\"{path}\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                });
                p?.WaitForExit(15000);
            }
            catch
            {
                // sound is not critical
            }
        }
    }

    /// <summary>
    /// Client window on macOS. Focus and keys via AppleScript / System Events (the terminal needs
    /// the Accessibility permission). On Windows this is done with user32.
    /// </summary>
    internal sealed class MacClientWindow : IUltimaClientWindow
    {
        private readonly Process process;
        private string lastTitle;

        public MacClientWindow(Process process)
        {
            this.process = process;
        }

        public IntPtr Handle => IntPtr.Zero;

        // The ClassicUO window cannot be renamed from outside on macOS, so the title set by scripts
        // (Title.Text) is shown in the title of the terminal window running Infusion (OSC 0 escape sequence).
        public void SetTitle(string title)
        {
            if (title == null || title == lastTitle)
                return;
            lastTitle = title;
            try
            {
                var clean = title.Replace('\u0007', ' ').Replace('\u001b', ' ');
                System.Console.Out.Write("\u001b]0;" + clean + "\u0007");
                System.Console.Out.Flush();
            }
            catch { }
        }

        public WindowBounds? GetBounds() => null;

        public void Focus()
        {
            if (process == null || process.HasExited)
                return;
            RunAppleScript($"tell application \"System Events\" to set frontmost of (first process whose unix id is {process.Id}) to true");
        }

        public void PressKey(char ch) => SendText(ch.ToString());

        public void PressKey(KeyCode keyCode)
        {
            int? macKey = keyCode switch
            {
                KeyCode.Left => 123,
                KeyCode.Right => 124,
                KeyCode.Down => 125,
                KeyCode.Up => 126,
                KeyCode.Home => 115,
                KeyCode.End => 119,
                KeyCode.PageUp => 116,
                KeyCode.PageDown => 121,
                _ => null
            };

            if (macKey.HasValue)
            {
                Focus();
                RunAppleScript($"tell application \"System Events\" to key code {macKey.Value}");
            }
        }

        public void SendText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            Focus();
            var escaped = text.Replace("\\", "\\\\").Replace("\"", "\\\"");
            RunAppleScript($"tell application \"System Events\" to keystroke \"{escaped}\"");
        }

        public void Click(int x, int y)
        {
            // not implemented (would need CGEvent on macOS)
        }

        private static void RunAppleScript(string script)
        {
            try
            {
                using var p = Process.Start(new ProcessStartInfo("/usr/bin/osascript", new[] { "-e", script })
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                });
                p?.WaitForExit(3000);
            }
            catch
            {
            }
        }
    }
}
