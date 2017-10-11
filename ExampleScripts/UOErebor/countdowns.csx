#load "Specs.csx"
#load "timers.csx"

using System;
using System.Collections.Generic;
using Infusion;
using Infusion.LegacyApi;

public static class Countdowns
{
    private class SpeechCountdown
    {
        public string TriggerText { get; }
        public string Name { get; }
        public TimeSpan Timeout { get; }
        public Color Color { get; }
        
        public SpeechCountdown(string triggerText, string name, TimeSpan timeout, Color color)
        {
            TriggerText = triggerText;
            Name = name;
            Timeout = timeout;
            Color = color;
        }
    }
    
    private class SkillCountdown
    {
        public Skill Skill { get; }
        public TimeSpan Timeout { get; }
        public Color Color { get; }
        
        public SkillCountdown(Skill skill, TimeSpan timeout, Color color)
        {
            Skill = skill;
            Timeout = timeout;
            Color = color;
        }
    }
    
    private class PotionCountdown
    {
        public ItemSpec BottleSpec { get; }
        public string Command { get; }
        public TimeSpan Timeout { get; }
        public Color Color { get; }
        
        public PotionCountdown(ItemSpec bottleSpec, string command, TimeSpan timeout, Color color)
        {
            this.BottleSpec = bottleSpec;
            this.Command = command;
            this.Timeout = timeout;
            this.Color = color;
        }
    }

    private static object speechReceivedCountdownsLock = new object();
    private static List<SpeechCountdown> speechReceivedCountdowns = new List<SpeechCountdown>();

    private static object skillRequestedCountdownsLock = new object();
    private static List<SkillCountdown> skillRequestedCountdowns = new List<SkillCountdown>();

    private static object potionCountdownsLock = new object();
    private static List<PotionCountdown> potionCountdowns = new List<PotionCountdown>();
    
    private static bool enabled = false;    

    static Countdowns()
    {
        UO.Events.SpeechReceived += (sender, args) => HandleSpeechReceived(args);
        UO.Events.SkillRequested += (sender, args) => HandleSkillRequested(args);
        UO.Events.SpeechRequested += (sender, args) => HandleSpeechRequested(args);
        UO.Events.ItemUseRequested += (sender, args) => HandleItemUseRequested(args);
    }

    private static void HandleItemUseRequested(ItemUseRequestedArgs args)
    {
        if (!enabled)
            return;
    
        var bottle = UO.Items[args.ItemId];
        if (bottle == null || !Specs.Bottle.Matches(bottle))
            return;
    
        lock (potionCountdownsLock)
        {
            foreach (var countdown in potionCountdowns)
            {
                UO.Log(countdown.Command);
                if (countdown.BottleSpec.Matches(bottle))
                {
                    Timers.AddTimer(countdown.Timeout, "potion", countdown.Color);
                    break;
                }
            }
        }
    }

    private static void HandleSpeechRequested(SpeechRequestedArgs args)
    {
        if (!enabled)
            return;

        lock (potionCountdownsLock)
        {
            foreach (var countdown in potionCountdowns)
            {
                if (args.Message != null && args.Message.Equals(countdown.Command, StringComparison.OrdinalIgnoreCase))
                {
                    Timers.AddTimer(countdown.Timeout, "potion", countdown.Color);
                    break;
                }
            }
        }
    }

    private static void HandleSkillRequested(Skill skill)
    {
        if (!enabled)
            return;
            
            lock (skillRequestedCountdownsLock)
            {
                foreach (var countdown in skillRequestedCountdowns)
                {
                    if (countdown.Skill == skill)
                    {
                        Timers.AddTimer(countdown.Timeout, skill.ToString(), countdown.Color);
                    }
                }
            }
    }

    private static void HandleSpeechReceived(JournalEntry journalEntry)
    {
        if (!enabled)
            return;
    
        lock (speechReceivedCountdownsLock)
        {
            foreach (var countdown in speechReceivedCountdowns)
            {
                if (journalEntry.Message.Equals(countdown.TriggerText))
                {
                    Timers.CancelTimer("spell");
                    Timers.AddTimer(countdown.Timeout, countdown.Name, countdown.Color); 
                }
            }
        }

        switch (journalEntry.Message)
        {
            case "Kouzlo se nezdarilo.":
            case "Nevidis na cil.":
                Timers.CancelTimer("spell");
                break;
        }
    }

    public static void AddSpellCountdown(string spell, TimeSpan timeout, Color color)
    {
        lock (speechReceivedCountdownsLock)
        {
            speechReceivedCountdowns.Add(new SpeechCountdown(spell, "spell",
                timeout, color));
        }
    }
    
    public static void AddSkillCountdown(Skill skill, TimeSpan timeout, Color color)
    {
        lock (skillRequestedCountdownsLock)
        {
            skillRequestedCountdowns.Add(new SkillCountdown(skill, timeout, color));
        }
    }
    
    public static void AddPotionCountdown(string command, ItemSpec bottleSpec, TimeSpan timeout, Color color)
    {
        lock (potionCountdownsLock)
        {
            potionCountdowns.Add(new PotionCountdown(bottleSpec, command, timeout, color));
        }
    }
    
    public static void Enable()
    {
        enabled = true;
    }
    
    public static void Disable()
    {
        enabled = false;
    }
}

UO.RegisterCommand("countdowns-enable", Countdowns.Enable);
UO.RegisterCommand("countdowns-disable", Countdowns.Disable);

Countdowns.AddPotionCountdown(".potionheal", Specs.HealPotions, TimeSpan.FromSeconds(20), Colors.Purple);
