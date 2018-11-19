using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal static class WavPlayer
    {
        public static void Play(string file)
        {
            var simpleSound = new SoundPlayer(file);
            simpleSound.PlaySync();
        }
    }
}
