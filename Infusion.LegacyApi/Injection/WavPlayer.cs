using Infusion.LegacyApi.Console;
using System.IO;
//using System.Media; FIXME

namespace Infusion.LegacyApi.Injection
{
    internal class WavPlayer
    {
        private readonly IConsole console;

        public WavPlayer(IConsole console)
        {
            this.console = console;
        }

        public void Play(string file)
        {
            if (!File.Exists(file))
            {
                //console.Error($"File {file} doesn't exist.");FIXME
                return;
            }

            //var simpleSound = new SoundPlayer(file);FIXME
            //simpleSound.PlaySync(); FIXME
        }
    }
}
