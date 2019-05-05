using Infusion.LegacyApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop
{
    internal sealed class SoundPlayer : ISoundPlayer
    {
        public void PlayFile(string path)
        {
            var simpleSound = new System.Media.SoundPlayer(path);
            simpleSound.PlaySync();
        }
    }
}
