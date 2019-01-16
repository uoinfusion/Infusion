using Infusion.LegacyApi.Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

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
                console.Error($"File {file} doesn't exist.");
                return;
            }

            var simpleSound = new SoundPlayer(file);
            simpleSound.PlaySync();
        }
    }
}
