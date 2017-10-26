#load "targeting.csx"

using System;
using Infusion;
using Infusion.LegacyApi;

public static class HitPointNotifier
{
    public static IPrintHitPointNotification Mode = HitPointNotificationModes.AboveAllMobiles;

    public static void Run()
    {
        var journal = UO.CreateEventJournal();

        journal.When<CurrentHealthUpdatedEvent>(OnHealthUpdated)
            .Incomming();
    }
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke(",hpnotify");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("hpnotify");
    }

    public static void Toggle()
    {
        if (UO.CommandHandler.IsCommandRunning("hpnotify-toggle"))
            Disable();
        else
            Enable();
    }

    private static void OnHealthUpdated(CurrentHealthUpdatedEvent args)
    {
        if (IsFirstStatusBarUpdate(args))
           return;
           
        // It seems like Sphere adds HP disregardig maximum HP. Then Sphere sends another
        // update packet which adjust HP to maximum value.
        if (IsHealingOverMaximumUpdate(args))
            return;
    
        var delta = args.UpdatedMobile.CurrentHealth - args.OldHealth;

        Mode.Print(delta, args.UpdatedMobile);
    }

    private static bool IsHealingOverMaximumUpdate(CurrentHealthUpdatedEvent args) =>
        args.UpdatedMobile.Id == UO.Me.PlayerId && args.UpdatedMobile.CurrentHealth > args.UpdatedMobile.MaxHealth;    

    private static bool IsFirstStatusBarUpdate(CurrentHealthUpdatedEvent args) =>
        args.OldHealth == 0 && args.UpdatedMobile.CurrentHealth == args.UpdatedMobile.MaxHealth;
}

public interface IPrintHitPointNotification
{
    void Print(int delta, Mobile updatedMobile);
}

public class OwnAndTargetAbovePlayerNotificationPrinter : IPrintHitPointNotification
{
    public Color MeColor { get; set; } = Colors.LightBlue;
    public Color OthersColor { get; set; } = Colors.Green;

    public void Print(int delta, Mobile mobile)
    {
        var deltaText = (delta > 0) ? "+" + delta.ToString() : delta.ToString();
        if (mobile.Id == Targeting.CurrentTarget)
        {
            if (!string.IsNullOrEmpty(mobile.Name))
            {
                UO.ClientPrint($"{mobile.Name}: {deltaText}/{mobile.CurrentHealth} %",
                    "hpnotify", UO.Me.PlayerId, UO.Me.BodyType, SpeechType.Speech,
                    OthersColor, log: false);
            }
            
        }
        else if (mobile.Id == UO.Me.PlayerId)
        {
            UO.ClientPrint($"{deltaText}/{mobile.CurrentHealth}",
                "hpnotify", UO.Me.PlayerId, UO.Me.BodyType, SpeechType.Speech,
                MeColor, log: false);                
        }
    }
}

public class AboveAllMobilesNotificationPrinter : IPrintHitPointNotification
{
    public Color EnemyColor { get; set; } = Colors.Green;
    public Color FriendColor { get; set; } = Colors.Blue;
    public Color MyColor { get; set; } = Colors.LightBlue;
    public Color PetsColor { get; set; } = Colors.Blue;

    public void Print(int delta, Mobile mobile)
    {
        var deltaText = (delta > 0) ? "+" + delta.ToString() : delta.ToString();

        Color textColor;
        
        if (mobile.Id == UO.Me.PlayerId)
        {
            textColor = MyColor;
        }
        else if (mobile.CanRename && Pets.MyPets.Contains(mobile.Id))
        {
            textColor = PetsColor;
        }
        else if (mobile.Notoriety == Notoriety.Friend)
        {
            textColor = FriendColor;
        }
        else
        {
            textColor = EnemyColor;
        }
        
    
        var currentHealthText = mobile.Id == UO.Me.PlayerId ?
            mobile.CurrentHealth.ToString() :
            $"{mobile.CurrentHealth} %";
    
        UO.ClientPrint($"{deltaText}/{currentHealthText}", "hpnotify",
        mobile.Id, mobile.Type, SpeechType.Speech, textColor, log: false);        
    }
}


public static class HitPointNotificationModes
{
    public static AboveAllMobilesNotificationPrinter AboveAllMobiles { get; } =
        new AboveAllMobilesNotificationPrinter();
    public static OwnAndTargetAbovePlayerNotificationPrinter OwnAndTargetAbovePlayer { get; } =
        new OwnAndTargetAbovePlayerNotificationPrinter();
}

UO.RegisterBackgroundCommand("hpnotify", HitPointNotifier.Run);
UO.RegisterCommand("hpnotify-toggle", HitPointNotifier.Toggle);
UO.RegisterCommand("hpnotify-enable", HitPointNotifier.Enable);
UO.RegisterCommand("hpnotify-disable", HitPointNotifier.Disable)