#load "colors.csx"
#load "Specs.csx"
#load "countdown.csx"

using System;

public static class Potions
{
    private static Countdown cooldownCountdown; 
    
    public static readonly TimeSpan SlowCooldown = TimeSpan.FromSeconds(20);
    public static readonly TimeSpan FastCooldown = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan NoDuration = TimeSpan.FromSeconds(0);
    
    public static readonly Potion HealPotion = new Potion(Specs.HealPotion, SlowCooldown, NoDuration);
    public static readonly Potion HealLesserPotion = new Potion(Specs.HealLesserPotion, SlowCooldown, NoDuration);
    public static readonly Potion HealGreaterPotion = new Potion(Specs.HealGreaterPotion, SlowCooldown, NoDuration);
    
    public static Color CooldownColor { get; set; } = Colors.Purple;
    public static CountdownStage[] CooldownStages = new[] { new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)) };
    
    private static Potion[] potions = new[]
    {
        HealLesserPotion, HealPotion, HealGreaterPotion,
    };
    
    public static void Enable()
    {
        UO.CommandHandler.Invoke("potions");
    }
    
    public static void Disable()
    {
        UO.CommandHandler.Terminate("potions");
    }
    
    public static void Toggle()
    {
        if (UO.CommandHandler.IsCommandRunning("potions"))
            Disable();
        else
            Enable();
    }

    public static void Run()
    {
        var journal = UO.CreateEventJournal();
        
        journal.When<ItemUseRequestedEvent>(HandleItemUseRequested)
            .Incomming();
    }

    private static void HandleItemUseRequested(ItemUseRequestedEvent args)
    {
        var bottle = UO.Items[args.ItemId];
        if (bottle == null || !Specs.Bottle.Matches(bottle))
            return;
    
        foreach (var potion in potions)
        {
            if (potion.Spec.Matches(bottle))
            {
                if (cooldownCountdown != null)
                    cooldownCountdown.Cancel();
                
                cooldownCountdown = new Countdown(potion.Cooldown, "potion cooldown", CooldownColor,  CooldownStages);
                cooldownCountdown.Start();
            }
        }
    }
}

public class Potion
{
    public TimeSpan Cooldown { get; }
    public TimeSpan Duration { get; }
    
    public ItemSpec Spec { get; }
    
    public Potion(ItemSpec spec, TimeSpan cooldown, TimeSpan duration)
    {
        this.Spec = spec;
        this.Cooldown = cooldown;
        this.Duration = duration;
    }
}

UO.RegisterBackgroundCommand("potions", Potions.Run);
UO.RegisterCommand("potions-enable", Potions.Enable);
UO.RegisterCommand("potions-disable", Potions.Disable);
UO.RegisterCommand("potions-toggle", Potions.Toggle);
