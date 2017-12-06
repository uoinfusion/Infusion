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
        var targetInfo = UO.AskForLocation();
        if (!targetInfo.HasValue)
        {
            UO.Log("Targeting cancelled");
            return;
        }
        
        UO.WarModeOff();
        UO.CastSpell(Spell.SummonCreature);

        if (UO.WaitForDialogBox("You lack ", "You can't make anything with what you have.", "You don't know that spell.") == null)
        {
            UO.Log("Cannot open summon dialog box");
            return;
        }
        
        UO.TriggerDialogBox(creatureName);
        
        UO.WaitForTarget();
        UO.Target(targetInfo.Value);
        
        bool spellFailed = false;
        
        eventJournal
            .When<MobileEnteredViewEvent>(
                e => Specs.Satan.Matches(e.Mobile) 
                        && (Location2D)e.Mobile.Location == (Location2D)targetInfo.Value.Location,
                e => spellFailed = false)
            .When<SpeechReceivedEvent>(
                e => IsFailMessage(e.Speech.Message), e => spellFailed = true)
            .WaitAny();
        
        if (spellFailed)
            return;
            
        UO.Say("all stay");
    }
    
    private static bool IsFailMessage(string message) =>
        message.Contains("Target is not in line of sight") || message.Contains("Kouzlo se nezdarilo.")
        || message.Contains("You can't make anything with what you have.");
}

UO.RegisterCommand("summon-satan", () => Necro.SummonCreature("Satan"));