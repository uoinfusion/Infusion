#load "Specs.csx"

using System;

public static class Summons
{
    private static EventJournal eventJournal = UO.CreateEventJournal();

    public static bool SummonCreature(Spell spell, MobileSpec creatureSpec, bool stop = true)
    {
        UO.Log($"Where do you want to cast {spell}"); 
        var targetInfo = UO.AskForLocation();
        if (!targetInfo.HasValue)
        {
            UO.Log("Targeting cancelled");
            return false;
        }
        
        UO.WarModeOff();
        UO.CastSpell(spell);
        
        UO.WaitForTarget();
        UO.Target(targetInfo.Value);
        
        bool spellFailed = false;
        
        eventJournal
            .When<MobileEnteredViewEvent>(
                e => creatureSpec.Matches(e.Mobile) 
                        && (Location2D)e.Mobile.Location == (Location2D)targetInfo.Value.Location,
                e => spellFailed = false)
            .When<SpeechReceivedEvent>(
                e => IsFailMessage(e.Speech.Message), e => spellFailed = true)
            .WhenTimeout(() => spellFailed = true)
            .WaitAny(TimeSpan.FromSeconds(10));
        
        if (spellFailed)
            return false;
        
        if (stop)
            UO.Say("all stay");
            
        return true;
    }

    public static bool SummonCreature(string creatureName, MobileSpec creatureSpec, bool stop = true)
    {
        UO.Log($"Where do you want to summon {creatureName}"); 
        var targetInfo = UO.AskForLocation();
        if (!targetInfo.HasValue)
        {
            UO.Log("Targeting cancelled");
            return false;
        }
        
        UO.WarModeOff();
        UO.CastSpell(Spell.SummonCreature);

        if (UO.WaitForDialogBox("You lack ", "You can't make anything with what you have.", "You don't know that spell.") == null)
        {
            UO.Log("Cannot open summon dialog box");
            return false;
        }
        
        if (!UO.TriggerDialogBox(creatureName))
        {
            UO.Log($"{creatureName} not in the dialog box.");
            UO.CloseDialogBox();
            return false;            
        }
        
        UO.WaitForTarget();
        UO.Target(targetInfo.Value);
        
        bool spellFailed = false;
        
        eventJournal
            .When<MobileEnteredViewEvent>(
                e => creatureSpec.Matches(e.Mobile) 
                        && (Location2D)e.Mobile.Location == (Location2D)targetInfo.Value.Location,
                e => spellFailed = false)
            .When<SpeechReceivedEvent>(
                e => IsFailMessage(e.Speech.Message), e => spellFailed = true)
            .WhenTimeout(() => spellFailed = true)
            .WaitAny(TimeSpan.FromSeconds(10));
        
        if (spellFailed)
            return false;
        
        if (stop)
            UO.Say("all stay");
            
        return true;
    }
    
    private static bool IsFailMessage(string message) =>
        message.Contains("Target is not in line of sight") || message.Contains("Kouzlo se nezdarilo.")
        || message.Contains("You can't make anything with what you have.") || message.Contains("You lack ")
        || message.Contains("You don't know that spell.");
}