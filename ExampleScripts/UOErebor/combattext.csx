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
    
    public static void Add(string text, Color? color = null)
    {
        if (color.HasValue)
            redirections.Add(new CombatTextRedirection(text, color.Value));
        else
            redirections.Add(new CombatTextRedirection(text));
    }

    private static void OnSpeechReceived(JournalEntry entry)
    {
        foreach (var redirection in redirections)
        {
            if (entry.Text.IndexOf(redirection.Text, 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var color = redirection.Color ?? entry.Color; 
                UO.ClientPrint(redirection.Text, entry.Name, UO.Me.PlayerId, UO.Me.BodyType,
                    SpeechType.Speech, color, false);
            }
        }
    }

    private class CombatTextRedirection
    {
        public string Text { get; }
        public Color? Color { get; }
        
        public CombatTextRedirection(string text, Color color)
        {
            Text = text;
            Color = color;
        }

        public CombatTextRedirection(string text)
        {
            Text = text;
        }
    }
}

UO.RegisterBackgroundCommand("combattext", CombatText.Run);
UO.RegisterCommand("combattext-enable", CombatText.Enable);
UO.RegisterCommand("combattext-disable", CombatText.Disable);
