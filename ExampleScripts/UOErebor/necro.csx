#load "Specs.csx"
#load "party.csx"
#load "TwoStateAbility.csx"
#load "summons.csx"

using System;

public static class Necro
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static TwoStateAbility DarkPower { get; } = new TwoStateAbility(".darkpower",
        "Nyni ti muze byt navracena spotrebovana mana.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.", new[] { "Jsi prilis vycerpany udrzovanim stitu." });
    
    private static bool IsFailMessage(string message) =>
        message.Contains("Target is not in line of sight") || message.Contains("Kouzlo se nezdarilo.")
        || message.Contains("You can't make anything with what you have.");
        
    public static bool SummonSatan() => Summons.SummonCreature("Satan", Specs.Satan);
    public static bool SummonTemnyVampir() => Summons.SummonCreature("Temny Vampir", Specs.TemnyVampir);
    public static bool SummonMumie() => Summons.SummonCreature("Mummy", Specs.Mumie);
    public static bool SummonLicheLord() => Summons.SummonCreature("Liche Lord", Specs.LicheLord);
    public static bool SummonHorse() => Summons.SummonCreature("Bone Horse", Specs.Horse);
    public static bool SummonLucistnik() => Summons.SummonCreature("Skeleton Archer", Specs.KostlivyLucistnik);
}

UO.RegisterCommand("summon-satan", () => Necro.SummonSatan());
UO.RegisterCommand("summon-temnyvampir", () => Necro.SummonTemnyVampir());
UO.RegisterCommand("summon-mumie", () => Necro.SummonMumie());
UO.RegisterCommand("summon-lichelord", () => Necro.SummonLicheLord());
UO.RegisterCommand("summon-horse", () => Necro.SummonHorse());
UO.RegisterCommand("summon-lucistnik", () => Necro.SummonLucistnik());