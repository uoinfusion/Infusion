using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.LegacyApi
{
    public interface ISoundPlayer
    {
        void PlayFile(string path);
    }

    public sealed class NullSoundPlayer : ISoundPlayer
    {
        public void PlayFile(string path) { /* just do nothing */ }
    }
}
