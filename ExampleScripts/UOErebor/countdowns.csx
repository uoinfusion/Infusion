#load "timers.csx"

using System;
using System.Collections.Generic;
using Infusion;
using Infusion.LegacyApi;

public static class Countdowns
{
    private struct SpeechCountdown
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
    
    private struct SkillCountdown
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

    private static object speechReceivedCountdownsLock = new object();
    private static List<SpeechCountdown> speechReceivedCountdowns = new List<SpeechCountdown>();
    private static object skillRequestedCountdownsLock = new object();
    private static List<SkillCountdown> skillRequestedCountdowns = new List<SkillCountdown>();
    
    private static bool enabled = false;    

    static Countdowns()
    {
        UO.Events.SpeechReceived += (sender, args) => HandleSpeechReceived(args);
        UO.Events.SkillRequested += (sender, args) => HandlSkillRequested(args);
    }

    private static void HandlSkillRequested(Skill skill)
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
