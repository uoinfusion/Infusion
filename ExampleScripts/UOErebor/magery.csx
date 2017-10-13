#load "Specs.csx"
#load "light.csx"
#load "meditation.csx"

using System;
using System.Globalization;
using Infusion.Commands;

public static class Magery
{
    public static void RecallHome()
    {
        UO.Say(".recallhome");
    }

    private static SpeechJournal recallHomeJournal = UO.CreateJournal();

    public static void Recall(Action castRecallAction)
    {
        Location3D startLocation = UO.Me.Location;
    
        bool failed;
        do
        {
            Meditation.Meditate(24);
            UO.Wait(250);
            failed = false;
            recallHomeJournal.Delete();
            castRecallAction();
    
            do
            {
                UO.Wait(100);
                if (recallHomeJournal.Contains("Kouzlo se nezdarilo"))
                {
                    failed = true;
                    Light.Check();
                    break;
                }
                if (recallHomeJournal.Contains("You don't know that spell."))
                {
                    throw new CommandInvocationException("Cannot cast recall!");
                }            
            }
            while (UO.Me.Location == startLocation);
        } while (failed);
        
        UO.ClientPrint("Waiting for changed location finished.");
    }
    
    public static void RecallHomeCommand()
    {
        Recall(() => UO.Say(".recallhome"));
    }
    
    public static void RecallTo(uint runeId)
    {
        var rune = UO.Items[runeId];
        if (rune == null)
            throw new CommandInvocationException($"Cannot find rune {runeId}"); 
        
        RecallTo(rune);
    }
    
    public static void RecallTo(Item rune)
    {
        var recallScroll = UO.Items
            .Matching(Specs.ScrollRecall)
            .InContainer(UO.Me.BackPack)
            .FirstOrDefault();
        
        if (recallScroll != null)
            UO.Use(recallScroll);
        else
        {
            UO.ClientPrint("No recall scolls found in backpack, trying to cast the spell from a spell book");
            UO.CastSpell(Spell.Recall);
        }
    
        UO.WaitForTarget();
        UO.Target(rune);
    }
    
    public static void RecallToCommand(string parameters)
    {
        if (!uint.TryParse(parameters, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint runeId))
        {
            UO.Alert("Expecting hexadecimal id of a rune.");
            return;
        }
        
        Recall(() => RecallTo(runeId));
    }
}

UO.RegisterCommand("recallhome", Magery.RecallHomeCommand);
UO.RegisterCommand("recallto", Magery.RecallToCommand);
