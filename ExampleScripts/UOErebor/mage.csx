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
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.", 
        new[] { "Jsi prilis vycerpany udrzovanim stitu." },
        new StateIndicator(0x80000000, 0x99000000, 2260, 0x20, 0));
        
    public static int FireChargerLevel => Chargers.FireLevel;
    public static int EnergyChargerLevel => Chargers.EnergyLevel;
    
    public static bool SummonDaemon() => Summons.SummonCreature(Spell.SummonDaemon, Specs.Daemon);
    public static bool SummonElementalVzduchu() => Summons.SummonCreature(Spell.SummonAirElemental, Specs.ElementalVzduchu);
    public static bool SummonElementalZeme() => Summons.SummonCreature(Spell.SummonEarthElemental, Specs.ElementalZeme);
    public static bool SummonElementalOhne() => Summons.SummonCreature(Spell.SummonFireElemental, Specs.ElementalOhne);
    public static bool SummonElementalVody() => Summons.SummonCreature(Spell.SummonWaterElemental, Specs.ElementalVody);
}

UO.RegisterCommand("summon-daemon", () => Mage.SummonDaemon());
UO.RegisterCommand("summon-elemvzduchu", () => Mage.SummonElementalVzduchu());
UO.RegisterCommand("summon-elemvzeme", () => Mage.SummonElementalZeme());
UO.RegisterCommand("summon-elemohne", () => Mage.SummonElementalOhne());
UO.RegisterCommand("summon-elemvody", () => Mage.SummonElementalVody());
