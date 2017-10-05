#load "targeting.csx"

using System;

public static class HitPointNotifier
{
    public static HitPointNotificationPrinter Mode = HitPointNotificationModes.AboveAllMobiles;

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
        }
    }

    public static void Toggle()
    {
        if (enabled)
        {
            Disable();
            UO.Log("HP notification disabled");
        }
        else
        {
            Enable();
            UO.Log("HP notification enabled");
        }
    }

    internal static void OnHealthUpdated(object sender, CurrentHealthUpdatedArgs args)
    {
        Mode.Print(args);
    }
}

public static class HitPointNotificationModes
{
    public static HitPointNotificationPrinter AboveAllMobiles { get; }
        = new HitPointNotificationPrinter((delta, mobile) =>
        {
            var currentHealthText = mobile.Id == UO.Me.PlayerId ?
                mobile.CurrentHealth.ToString() :
                $"{mobile.CurrentHealth} %";
        
            UO.ClientPrint($"{delta}/{currentHealthText}", "hpnotify",
            mobile.Id, mobile.Type, SpeechType.Speech, Colors.Green, log: false);        
        });
     
     public static HitPointNotificationPrinter OwnAndTargetAbovePlayer { get; }
        = new HitPointNotificationPrinter((delta, mobile) =>
        {     
            var deltaText = (delta > 0) ? "+" + delta.ToString() : delta.ToString();
            if (mobile.Id == Targeting.CurrentTarget)
            {
                if (!string.IsNullOrEmpty(mobile.Name))
                {
                    UO.ClientPrint($"{mobile.Name}: {deltaText}/{mobile.CurrentHealth} %",
                        "hpnotify", UO.Me.PlayerId, UO.Me.BodyType, SpeechType.Speech,
                        Colors.Green, log: false);
                }
                
            }
            else if (mobile.Id == UO.Me.PlayerId)
            {
                UO.ClientPrint($"{deltaText}/{mobile.CurrentHealth}",
                    "hpnotify", UO.Me.PlayerId, UO.Me.BodyType, SpeechType.Speech,
                    Colors.Red, log: false);                
            }
        });
}

public class HitPointNotificationPrinter
{
    private Action<int, Mobile> printAction;

    public HitPointNotificationPrinter(Action<int, Mobile> printAction)
    {
        this.printAction = printAction;
    }
    
    public void Print(CurrentHealthUpdatedArgs args)
    {
        if (args.OldHealth == 0 && args.UpdatedMobile.CurrentHealth == args.UpdatedMobile.MaxHealth)
            return;
    
        var delta = args.UpdatedMobile.CurrentHealth - args.OldHealth;
        
        printAction(delta, args.UpdatedMobile);
    }
}

UO.RegisterCommand("hpnotify", HitPointNotifier.Toggle);