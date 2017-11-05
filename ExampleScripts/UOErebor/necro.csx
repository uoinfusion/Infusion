#load "Specs.csx"
#load "party.csx"
#load "TwoStateAbility.csx"

using System;

public static class Necro
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static TwoStateAbility DarkPower { get; } = new TwoStateAbility(".darkpower",
        "Nyni ti muze byt navracena spotrebovana mana.", "Jsi zpatky v normalnim stavu.");
    public static TwoStateAbility StandCast { get; } = new TwoStateAbility(".standcast",
        "Nyni budes kouzlit s vetsim usilim.", "Tve usili polevilo.");
    
    
    public static void SummonCreature(string creatureName)
    {
        UO.Log($"Where do you want to summon {creatureName}"); 
        var targetInfo = UO.Info();
        if (!targetInfo.HasValue)
        {
            UO.Log("Targeting cancelled");
            return;
        }
        
        UO.WarModeOff();
        UO.CastSpell(Spell.SummonCreature);
        UO.WaitForDialogBox();
        UO.TriggerDialogBox(creatureName);
        UO.WaitForTarget();
        UO.Target(targetInfo.Value.Location);
        
        bool spellFailed = false;
        
        eventJournal
            .When<MobileEnteredViewEvent>(
                e => Specs.Satan.Matches(e.Mobile) 
                        && (Location2D)e.Mobile.Location == (Location2D)targetInfo.Value.Location,
                e => { })
            .When<SpeechReceivedEvent>(e => e.Speech.Message.Contains("Kouzlo se nezdarilo."), e => spellFailed = true)
            .WaitAny();
        
        if (spellFailed)
            return;
            
        UO.Say("all stay");
    }
}

UO.RegisterCommand("summon-satan", () => Necro.SummonCreature("Satan"));