#load "colors.csx"
#load "Specs.csx"
#load "countdown.csx"

using System;
using System.Linq;
using Infusion.LegacyApi.Events;

public static class Potions
{
    private static Countdown cooldownCountdown; 
    
    public static readonly TimeSpan SlowCooldown = TimeSpan.FromSeconds(21);
    public static readonly TimeSpan FastCooldown = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan NoDuration = TimeSpan.FromSeconds(0);
    public static CountdownStage[] CooldownStages = new[] { new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1)) };
    public static CountdownStage[] ImportantDurationStages = new[]
    {
        new CountdownStage(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)),
        new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1))
    };
    public static CountdownStage[] NormalDurationStages = new[]
    {
        new CountdownStage(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(1))
    };
    public static Color CooldownColor { get; set; } = Colors.Purple;
    public static Color DurationColor { get; set; } = Colors.Purple;
    
    public static ScriptTrace Trace { get; } = UO.Trace.Create();
    
    public static readonly Potion HealPotion = new Potion(Specs.HealPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion HealLesserPotion = new Potion(Specs.HealLesserPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion HealGreaterPotion = new Potion(Specs.HealGreaterPotion, SlowCooldown, NoDuration, ".potionheal");
    public static readonly Potion RefreshLesserPotion = new Potion(Specs.RefreshLesserPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion RefreshPotion = new Potion(Specs.RefreshPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion RefreshGreaterPotion = new Potion(Specs.RefreshGreaterPotion, SlowCooldown, NoDuration, ".potionrefresh");
    public static readonly Potion InvisPotion = new Potion(Specs.InvisibilityPotion, SlowCooldown, TimeSpan.FromSeconds(190), ".potioninvis", "invis", ImportantDurationStages);
    public static readonly Potion ClevernessLesserPotion = new Potion(Specs.ClevernessLesserPotion, SlowCooldown, TimeSpan.FromSeconds(120), ".potionclever", "cleverness", NormalDurationStages);
    public static readonly Potion ClevernessPotion = new Potion(Specs.ClevernessPotion, SlowCooldown, TimeSpan.FromSeconds(330), ".potionclever", "cleverness", NormalDurationStages);
    public static readonly Potion ClevernessGreaterPotion = new Potion(Specs.ClevernessGreaterPotion, SlowCooldown, TimeSpan.FromSeconds(600), ".potionclever", "cleverness", NormalDurationStages);
    public static readonly Potion CureLesserPotion = new Potion(Specs.CureLesserPotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion CurePotion = new Potion(Specs.CurePotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion CureGreaterPotion = new Potion(Specs.CureGreaterPotion, SlowCooldown, NoDuration, ".potioncure");
    public static readonly Potion NightsightPotion = new Potion(Specs.NightsighPoition, FastCooldown, TimeSpan.FromSeconds(1800), ".potionnightsight", "nightsight", NormalDurationStages);
    public static readonly Potion StregthGreaterPotion = new Potion(Specs.StrengthGreaterPotion, SlowCooldown, TimeSpan.FromSeconds(600), ".potionstrength", "strength", NormalDurationStages);
    public static readonly Potion AgilityGreaterPotion = new Potion(Specs.AgilityGreaterPotion, SlowCooldown, TimeSpan.FromSeconds(600), ".potionagility", "agility", NormalDurationStages);
    public static readonly Potion MobilityPotion = new Potion(Specs.MobilityPotion, FastCooldown, TimeSpan.FromSeconds(30), ".potionmobility", "mobility", ImportantDurationStages);
    
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
	    StregthGreaterPotion, AgilityGreaterPotion, MobilityPotion,
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
    
    public static void Heal()
    {
        int deltaHP = UO.Me.MaxHealth - UO.Me.CurrentHealth;
        
        if (deltaHP <= 0)
        {
            UO.ClientPrint("Full HP, no need to drink heal potion.");
            return;
        }
        
        Item healPotion = null;
        if (deltaHP <= 30)
            healPotion = FindBackpackItem(Specs.HealLesserPotion) ?? FindBackpackItem(Specs.HealPotion);
        else if (deltaHP <= 50)
            healPotion = FindBackpackItem(Specs.HealPotion);
            
        if (healPotion == null)
            UO.Say(".potionheal");
        else
            UO.Use(healPotion);
    }
    
    private static Item FindBackpackItem(ItemSpec spec)
        => UO.Items.InBackPack(true).Matching(spec).FirstOrDefault();
    
    public static void Invis()
    {
        UO.WarModeOn();
        UO.WarModeOff();
        
        UO.Say(".potioninvis");
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
                if (UO.Items.InBackPack(true).Matching(potion.Spec).Any())
                {
                    Trace.Log("potion found in backpack");
                    StartPotion(potion);
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
                StartPotion(potion);
            }
        }
    }
    
    private static void StartPotion(Potion potion)
    {
        if (cooldownCountdown == null || cooldownCountdown.Expired)
        {
            potion.StartDuration();
        }
    
        if (cooldownCountdown != null)
            cooldownCountdown.Cancel();
        
        cooldownCountdown = new Countdown(potion.Cooldown, "potion cooldown", CooldownColor,  CooldownStages);
        cooldownCountdown.Start();
    }
}

public class Potion
{
    private Countdown potionDurationCountdown;

    public TimeSpan Cooldown { get; set; }
    public TimeSpan Duration { get; set; }
    public CountdownStage[] DurationStages { get; set; }
    
    public ItemSpec Spec { get; }
    public string Command { get; }
    public string Name { get; set; }
    
    public Potion(ItemSpec spec, TimeSpan cooldown, TimeSpan duration, string command, string name = null, CountdownStage[] durationStages = null)
    {
        this.Spec = spec;
        this.Cooldown = cooldown;
        this.Duration = duration;
        this.Command = command;
        this.DurationStages = durationStages;
        
        Name = name ?? "potion";
    }
    
    public void StartDuration()
    {
        if (Duration.TotalMilliseconds > 0)
        {
            if (potionDurationCountdown != null)
                potionDurationCountdown.Cancel();
        
            Potions.Trace.Log($"{Name} DurationStages length {this.DurationStages?.Length.ToString() ?? "null"}");
            var stages = this.DurationStages ?? Potions.NormalDurationStages;
            potionDurationCountdown = new Countdown(Duration, Name, Potions.DurationColor, stages);
            potionDurationCountdown.Start();
            Potions.Trace.Log($"{Name} duration ({Duration}) countdown started with {potionDurationCountdown.Stages.Length} stages");
        }            
    }
}

UO.RegisterBackgroundCommand("potions", Potions.Run);
UO.RegisterCommand("potions-enable", Potions.Enable);
UO.RegisterCommand("potions-disable", Potions.Disable);
UO.RegisterCommand("potions-toggle", Potions.Toggle);
UO.RegisterCommand("potioninvis", Potions.Invis);
UO.RegisterCommand("potionheal", Potions.Heal)