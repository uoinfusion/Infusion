using System.Collections.Generic;

namespace Infusion.LegacyApi.Filters
{
    public interface ISoundFilter
    {
        void SetFilteredSounds(IEnumerable<SoundId> sounds);
        void Disable();
    }
}