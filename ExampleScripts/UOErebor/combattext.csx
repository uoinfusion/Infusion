using System;

public static class CombatText
{
    public static CombatTextRedirection[] Redirections { get; set; } = { };
    private static bool enabled = false;

    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        
        journal.When<SpeechReceivedEvent>(e => OnSpeechReceived(e.Speech))
            .HandleIncomming();
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke(",combattext");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("combattext");
    }

    private static void OnSpeechReceived(JournalEntry entry)
    {
        foreach (var redirection in Redirections)
        {
            if (entry.Text.IndexOf(redirection.Text, 0, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                UO.ClientPrint(redirection.Text, entry.Name, UO.Me.PlayerId, UO.Me.BodyType,
                    SpeechType.Speech, redirection.Color, false);
            }
        }
    }
}

public class CombatTextRedirection
{
    public string Text { get; }
    public Color Color { get; }
    
    public CombatTextRedirection(string text, Color color)
    {
        Text = text;
        Color = color;
    }
}

UO.RegisterBackgroundCommand("combattext", CombatText.Run);
UO.RegisterCommand("combattext-enable", CombatText.Enable);
UO.RegisterCommand("combattext-disable", CombatText.Disable);
