using System;

public static class CombatText
{
    public static CombatTextRedirection[] Redirections { get; set; } = { };
    private static bool enabled = false;

    public static void Enable()
    {
        if (!enabled)
        {
            UO.Events.SpeechReceived += OnSpeechReceived;
            enabled = true;
        }
    }

    public static void Disable()
    {
        if (enabled)
        {
            UO.Events.SpeechReceived -= OnSpeechReceived;
            enabled = false;
        }
    }
    
    public static void Toggle()
    {
        if (!enabled)
        {
            Enable();
            UO.Log("Combat text enabled");
        }
        else
        {
            Disable();
            UO.Log("Combat text disabled");
        }
    }

    private static void OnSpeechReceived(object sender, JournalEntry entry)
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

UO.RegisterCommand("combattext-enable", CombatText.Enable);
UO.RegisterCommand("combattext-disable", CombatText.Disable);
UO.RegisterCommand("combattext-toggle", CombatText.Toggle);
