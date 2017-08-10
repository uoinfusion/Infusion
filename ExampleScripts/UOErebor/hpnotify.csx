public static class HitPointNotifier
{
    private static bool enabled;

    public static void Enable()
    {
        if (!enabled)
        {
            UO.Events.HealthUpdated += OnHealthUpdated;
            enabled = true;

            UO.ClientPrint("HP notification enabled");
        }
    }

    public static void Disable()
    {
        if (enabled)
        {
            UO.Events.HealthUpdated -= OnHealthUpdated;
            enabled = false;
            UO.ClientPrint("HP notification disabled");
        }
    }

    public static void Toggle()
    {
        if (enabled)
            Disable();
        else
            Enable();
    }

    internal static void OnHealthUpdated(object sender, CurrentHealthUpdatedArgs args)
    {
        var delta = args.UpdatedMobile.CurrentHealth - args.OldHealth;
        if (delta != 0)
        {
            var color = delta > 0 ? Colors.Blue : Colors.Green;
            UO.ClientPrint($"{delta}/{args.UpdatedMobile.CurrentHealth}", "hpnotify",
                args.UpdatedMobile.Id,
                    args.UpdatedMobile.Type, SpeechType.Speech, color, log: false);
        }
    }
}

UO.RegisterCommand("hpnotify", HitPointNotifier.Toggle);