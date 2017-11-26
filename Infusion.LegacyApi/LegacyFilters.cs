using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.LegacyApi.Filters;

namespace Infusion.LegacyApi
{
    public class LegacyFilters
    {
        internal LegacyFilters(StaminaFilter staminaFilter, LightObserver lightObserver, WeatherObserver weatherObserver, SoundObserver soundObserver)
        {
            Stamina = staminaFilter;
            Light = lightObserver;
            Weather = weatherObserver;
            Sound = soundObserver;
        }

        public IStaminaFilter Stamina { get; }
        public ILightFilter Light { get; }
        public IWeatherFilter Weather { get; }
        public ISoundFilter Sound { get; }
    }
}
