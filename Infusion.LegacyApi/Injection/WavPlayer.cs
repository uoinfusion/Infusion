using Infusion.LegacyApi.Console;
using System.IO;
using System.Threading;

namespace Infusion.LegacyApi.Injection
{
    internal class WavPlayer
    {
        private readonly IConsole console;
        private readonly ISoundPlayer soundPlayer;

        public WavPlayer(IConsole console, ISoundPlayer soundPlayer)
        {
            this.console = console;
            this.soundPlayer = soundPlayer;
        }

        public void Play(string file)
        {
            if (!File.Exists(file))
            {
                console.Error($"File {file} doesn't exist.");
                return;
            }

            soundPlayer.PlayFile(file);
        }
    }
}
