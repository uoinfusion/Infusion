using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.LegacyApi.Filters;

namespace Infusion.LegacyApi
{
    public sealed class LegacyFilters
    {
        internal LegacyFilters(StaminaFilter staminaFilter, LightObserver lightObserver, WeatherObserver weatherObserver,
            SoundObserver soundObserver, ShapeshiftingFilter shapeShifter, WalkingObserver walkingObserver)
        {
            Stamina = staminaFilter;
            Light = lightObserver;
            Weather = weatherObserver;
            Sound = soundObserver;
            ShapeShifter = shapeShifter;
            Walking = walkingObserver;
        }

        public IStaminaFilter Stamina { get; }
        public ILightFilter Light { get; }
        public IWeatherFilter Weather { get; }
        public ISoundFilter Sound { get; }
        public IShapeshiftingFilter ShapeShifter { get; }
        public IWalkingFilter Walking { get; }
    }
}
