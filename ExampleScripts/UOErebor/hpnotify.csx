public static class HitPointNotifier
{
    private static bool enabled;

    public static void Enable()
    {
        // if notification is already enabled then do nothing
        if (!enabled)
        {
            // This installs OnHealthUpdated subscribes to HealthUpdated event.
            // It means that Infusion calls OnHealthUpdated method every time when
            // server updates health (hit points) of any mobile including the player.            
            // See https://docs.microsoft.com/en-us/dotnet/standard/events/ for
            // more information about events in C#.
            UO.Events.HealthUpdated += OnHealthUpdated;
            
            enabled = true;

            UO.ClientPrint("HP notification enabled");
        }
    }

    public static void Disable()
    {
        if (enabled)
        {
            // The notification is disabled by unsubscribing the OnHealthUpdated method
            // from HealthUpdated event.
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
        // Event arguments contain reference to the updated mobile, so you can
        // have any information about the updated mobile, not only the current health
        // (args.UpdatedMobile.CurrentHealth). Arguments contain original health value
        // (args.OldHealth), so you can easilly calculate the delta value:
        var delta = args.UpdatedMobile.CurrentHealth - args.OldHealth;

        // NOTE: delta cannot be 0 at this point, otherwise Infusion doesn't call the event handler.
        var color = delta > 0 ? Colors.Blue : Colors.Green;

        // This defines the text that will the mobile say:
        // $"{delta}/{args.UpdatedMobile.CurrentHealth}"
        // For more information about string formating in C# see
        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/interpolated-strings
        // Important parameter value is args.UpdatedMobile.Id it defines who says the text.
        UO.ClientPrint($"{delta}/{args.UpdatedMobile.CurrentHealth}", "hpnotify",
            args.UpdatedMobile.Id,
                args.UpdatedMobile.Type, SpeechType.Speech, color, log: false);
    }
}

UO.RegisterCommand("hpnotify", HitPointNotifier.Toggle);