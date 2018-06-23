#load "Specs.csx"
#load "TwoStateAbility.csx"
#load "chargers.csx"
#load "summons.csx"

using System;
using System.Linq;
using Infusion.Gumps;
using Infusion.LegacyApi.Events;

public static class Mage
{
    public static TwoStateAbility Concentration { get; } = new TwoStateAbility(".concentration",
        "Nyni jsi schopen kouzlit za mene many.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
        
    public static int FireChargerLevel => Chargers.FireLevel;
    public static int EnergyChargerLevel => Chargers.EnergyLevel;    
}
