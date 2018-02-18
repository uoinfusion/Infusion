#load "colors.csx"
#load "Specs.csx"
#load "countdown.csx"

using System;
using System.Linq;
using Infusion.LegacyApi.Events;

public static class Potions
{
    private static Countdown cooldownCountdown; 
    
    public static readonly TimeSpan SlowCooldown = TimeSpan.FromSeconds(20);
    public static readonly TimeSpan FastCooldown = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan NoDuration = TimeSpan.FromSeconds(0);
    
    public static ScriptTrace Trace { get; } = UO.Trace.Create();
    
    public static readonly Potion HealPotion = new Potion(Specs.HealPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion HealLesserPotion = new Potion(Specs.HealLesserPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion HealGreaterPotion = new Potion(Specs.HealGreaterPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion RefreshLesserPotion = new Potion(Specs.RefreshLesserPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion RefreshPotion = new Potion(Specs.RefreshPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion RefreshGreaterPotion = new Potion(Specs.RefreshGreaterPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion InvisPotion = new Potion(Specs.InvisibilityPotion, SlowCooldown, TimeSpan.FromSeconds(220), ".potioninvis");
    public static readonly Potion ClevernessLesserPotion = new Potion(Specs.ClevernessLesserPotion, SlowCooldown, NoDuration, ".potionclever");
    public static readonly Potion ClevernessPotion = new Potion(Specs.ClevernessPotion, SlowCooldown, NoDuration, ".potionclever");
    public static readonly Potion ClevernessGreaterPotion = new Potion(Specs.ClevernessGreaterPotion, SlowCooldown, NoDuration, ".potionclever");
    public static readonly Potion CureLesserPotion = new Potion(Specs.CureLesserPotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion CurePotion = new Potion(Specs.CurePotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion CureGreaterPotion = new Potion(Specs.CureGreaterPotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion NightsightPotion = new Potion(Specs.NightsighPoition, FastCooldown, NoDuration, ".potionnightsight");
    
    public static Color CountdownColor { get; set; } = Colors.Purple;
    public static CountdownStage[] CooldownStages = new[] { new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)) };
    
    // Order of potions in potions array is important!
    // It is because command handling and potion consumption order on server.
    // Potion command tries to consume highest possible potion available, so greater potions
    // have to be registered first, otherwise there can be a mismatch between the cooldown in this script
    // and the cooldown on server.
    private static Potion[] potions = new[]
    {
        HealGreaterPotion, HealPotion, HealLesserPotion,
        RefreshGreaterPotion, RefreshPotion, RefreshLesserPotion,
        ClevernessGreaterPotion, ClevernessPotion, ClevernessLesserPotion,
        CureGreaterPotion, CurePotion, CureLesserPotion,
        InvisPotion, NightsightPotion,
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
            .When<SpeechRequestedEvent>(HandleCommands)
            .Incomming();
    }

    private static void HandleCommands(SpeechRequestedEvent args)
    {
        if (!args.Message.StartsWith("."))
            return;
    
        foreach (var potion in potions)
        {
            Trace.Log($"potion command: {potion.Command}, command requested: {args.Message}");
            if (args.Message.Equals(potion.Command, StringComparison.OrdinalIgnoreCase))
            {
                Trace.Log("command found");
                if (UO.Items.InBackPack().Matching(potion.Spec).Any())
                {
                    Trace.Log("potion found in backpack");
                    StartCountdown(potion);
                }
                else
                {
                    Trace.Log("potion not found in backpack");
                }
            }
            else
            {
                Trace.Log("command not found");
            }
        }
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
                StartCountdown(potion);
            }
        }
    }
    
    private static void StartCountdown(Potion potion)
    {
        if (cooldownCountdown != null)
            cooldownCountdown.Cancel();
        
        cooldownCountdown = new Countdown(potion.Cooldown, "potion cooldown", CountdownColor,  CooldownStages);
        cooldownCountdown.Start();
    }
}

public class Potion
{
    public TimeSpan Cooldown { get; }
    public TimeSpan Duration { get; }
    
    public ItemSpec Spec { get; }
    public string Command { get; }
    
    public Potion(ItemSpec spec, TimeSpan cooldown, TimeSpan duration, string command)
    {
        this.Spec = spec;
        this.Cooldown = cooldown;
        this.Duration = duration;
        this.Command = command;
    }
}

UO.RegisterBackgroundCommand("potions", Potions.Run);
UO.RegisterCommand("potions-enable", Potions.Enable);
UO.RegisterCommand("potions-disable", Potions.Disable);
UO.RegisterCommand("potions-toggle", Potions.Toggle);
