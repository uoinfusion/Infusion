using System;
using System.Collections.Generic;

public static class CombatText
{
    private static List<CombatTextRedirection> redirections = new List<CombatTextRedirection>();
    private static bool enabled = false;

    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        
        journal.When<SpeechReceivedEvent>(e => OnSpeechReceived(e.Speech))
            .Incomming();
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke("combattext");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("combattext");
    }
    
    public static void Add(string text, Color color)
    {
        redirections.Add(new CombatTextRedirection(text, color));
    }

    private static void OnSpeechReceived(JournalEntry entry)
    {
        foreach (var redirection in redirections)
        {
            if (entry.Text.IndexOf(redirection.Text, 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                UO.ClientPrint(redirection.Text, entry.Name, UO.Me.PlayerId, UO.Me.BodyType,
                    SpeechType.Speech, redirection.Color, false);
            }
        }
    }

    private class CombatTextRedirection
    {
        public string Text { get; }
        public Color Color { get; }
        
        public CombatTextRedirection(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }
}

UO.RegisterBackgroundCommand("combattext", CombatText.Run);
UO.RegisterCommand("combattext-enable", CombatText.Enable);
UO.RegisterCommand("combattext-disable", CombatText.Disable);
