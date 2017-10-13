#load "Specs.csx"
#load "countdown.csx"

using System;

public static class Potions
{
    private static bool countdownEnabled;
    private static Countdown cooldownCountdown; 
    
    public static readonly TimeSpan SlowCooldown = TimeSpan.FromSeconds(20);
    public static readonly TimeSpan FastCooldown = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan NoDuration = TimeSpan.FromSeconds(0);
    
    public static readonly Potion HealPotion = new Potion(Specs.HealPotion, FastCooldown, NoDuration);
    public static readonly Potion HealLesserPotion = new Potion(Specs.HealLesserPotion, FastCooldown, NoDuration);
    public static readonly Potion HealGreaterPotion = new Potion(Specs.HealGreaterPotion, FastCooldown, NoDuration);
    
    public static Color CooldownColor { get; set; } = Colors.Purple;
    public static CountdownStage[] CooldownStages = new[] { new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)) };
    
    private static Potion[] potions = new[]
    {
        HealLesserPotion, HealPotion, HealGreaterPotion,
    };

    static Potions()
    {
        UO.Events.ItemUseRequested += (sender, args) => HandleItemUseRequested(args);
    }
    
    private static void HandleItemUseRequested(ItemUseRequestedArgs args)
    {
        if (!countdownEnabled)
            return;
    
        var bottle = UO.Items[args.ItemId];
        if (bottle == null || !Specs.Bottle.Matches(bottle))
            return;
    
        foreach (var potion in potions)
        {
            if (potion.Spec.Matches(bottle))
            {
                if (cooldownCountdown != null)
                    cooldownCountdown.Cancel();
                
                cooldownCountdown = new Countdown(potion.Cooldown, "potion", CooldownColor,  CooldownStages);
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