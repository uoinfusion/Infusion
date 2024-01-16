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

    private static SpeechJournal recallHomeJournal = UO.CreateSpeechJournal();

    public static void Recall(Action castRecallAction, bool meditate = true)
    {
        UO.Say(".resync");
        UO.Wait(250);
        Location2D startLocation = UO.Me.Location;
    
        bool failed;
        Location2D currentLocation = UO.Me.Location;
        do
        {
            if (!meditate)
            {
                while (UO.Me.CurrentMana < 24)
                {
                    UO.Wait(1000);
                }
            }
            else
                Meditation.Meditate(24);
                
            UO.Wait(250);
            failed = false;
            recallHomeJournal.Delete();
            castRecallAction();
            UO.Log("Waiting for changed location.");
    
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
                currentLocation = UO.Me.Location;
            }
            while (currentLocation == startLocation);
        } while (failed);
        
        UO.Log($"Recall {startLocation} -> {currentLocation} finished.");
    }
    
    public static void RecallHomeCommand()
    {
        RecallHome(true);
    }
    
    public static void RecallHome(bool meditate = true)
    {
        Recall(() => UO.Say(".recallhome"), meditate);
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
    
        UO.Log("Waiting for target.");
        UO.WaitForTarget();
        UO.Log("Target rune.");
        UO.Target(rune);
    }
    
    public static void RecallToCommand(string parameters)
    {
        if (!uint.TryParse(parameters, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint runeId))
        {
            throw new CommandInvocationException("Expecting hexadecimal id of a rune.");
        }
                
        Recall(() => RecallTo(runeId));
    }
}

UO.RegisterCommand("recallhome", Magery.RecallHomeCommand);
UO.RegisterCommand("recallto", Magery.RecallToCommand);
