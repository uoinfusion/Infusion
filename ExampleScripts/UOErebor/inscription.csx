#load "banking.csx"
#load "common.csx"
#load "light.csx"
#load "afk.csx"
#load "meditation.csx"
#load "eating.csx"
#load "items.csx"
#load "warehouse.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class InscriptionMenu
{
    public static InscriptionScroll Weaken = new InscriptionScroll(Specs.ScrollWeaken, Specs.BlankScroll, "Kruh 1", "Weaken Scroll");
    public static InscriptionScroll NightSight = new InscriptionScroll(Specs.ScrollNightSight, Specs.BlankScroll, "Kruh 1", "Night Sight Scroll");
    public static InscriptionScroll MagicArrow = new InscriptionScroll(Specs.ScrollMagicArrow, Specs.BlankScroll, "Kruh 1", "Magic Arrow Scroll");
    public static InscriptionScroll Feeblemind = new InscriptionScroll(Specs.ScrollFeeblemind, Specs.BlankScroll, "Kruh 1", "Feeblemind Scroll");
    public static InscriptionScroll Clumsy = new InscriptionScroll(Specs.ScrollClumsy, Specs.BlankScroll, "Kruh 1", "Clumsy Scroll");
    public static InscriptionScroll ReactiveArmor = new InscriptionScroll(Specs.ScrollReactiveArmor, Specs.BlankScroll, "Kruh 1", "Reactive Armor Scr");
    public static InscriptionScroll Heal = new InscriptionScroll(Specs.ScrollHeal, Specs.BlankScroll, "Kruh 1", "Heal Scroll");

    public static InscriptionScroll Strength = new InscriptionScroll(Specs.ScrollStrength, Specs.BlankScroll, "Kruh 2", "Strength Scroll");
    public static InscriptionScroll Harm = new InscriptionScroll(Specs.ScrollHarm, Specs.BlankScroll, "Kruh 2", "Harm Scroll");
    public static InscriptionScroll Cure = new InscriptionScroll(Specs.ScrollCure, Specs.BlankScroll, "Kruh 2", "Cure Scroll");
    public static InscriptionScroll Cunnig = new InscriptionScroll(Specs.ScrollCunning, Specs.BlankScroll, "Kruh 2", "Cunning Scroll");
    public static InscriptionScroll Agility = new InscriptionScroll(Specs.ScrollAgility, Specs.BlankScroll, "Kruh 2", "Agility Scroll");
    public static InscriptionScroll Protection = new InscriptionScroll(Specs.ScrollProtection, Specs.BlankScroll, "Kruh 2", "Protection Scroll");

    public static InscriptionScroll WallOfStone = new InscriptionScroll(Specs.ScrollWallOfStone, Specs.BlankScroll, "Kruh 3", "Wall of Stone Scro");
    public static InscriptionScroll Poison = new InscriptionScroll(Specs.Poison, Specs.BlankScroll, "Kruh 3", "Poison Scroll");
    public static InscriptionScroll Fireball = new InscriptionScroll(Specs.ScrollFireball, Specs.BlankScroll, "Kruh 3", "Fireball Scroll");
    public static InscriptionScroll Teleport = new InscriptionScroll(Specs.ScrollTeleport, Specs.BlankScroll, "Kruh 3", "Teleport Scroll");
    public static InscriptionScroll Bless = new InscriptionScroll(Specs.ScrollBless, Specs.BlankScroll, "Kruh 3", "Bless Scroll");

    public static InscriptionScroll Recall = new InscriptionScroll(Specs.ScrollRecall, Specs.BlankChestnutScroll, "Kruh 4", "Recall");
    public static InscriptionScroll ManaDrain = new InscriptionScroll(Specs.ScrollManaDrain, Specs.BlankChestnutScroll, "Kruh 4", "ManaDrain Scroll");
    public static InscriptionScroll GreaterHeal = new InscriptionScroll(Specs.ScrollGreaterHeal, Specs.BlankChestnutScroll, "Kruh 4", "Greater Heal Scrol");
    public static InscriptionScroll ArchProtection = new InscriptionScroll(Specs.ScrollArchprotection, Specs.BlankChestnutScroll, "Kruh 4", "Arch Protection Sc");
    public static InscriptionScroll ArchCure = new InscriptionScroll(Specs.ScrollArchcure, Specs.BlankChestnutScroll, "Kruh 4", "Archcure Scroll");
    public static InscriptionScroll Curse = new InscriptionScroll(Specs.ScrollCurse, Specs.BlankChestnutScroll, "Kruh 4", "Curse Scroll");

    public static InscriptionScroll SummonCreature = new InscriptionScroll(Specs.ScrollSummonCreature, Specs.BlankChestnutScroll, "Kruh 5", "Summon Creature Sc");
    public static InscriptionScroll Paralyze = new InscriptionScroll(Specs.ScrollParalyze, Specs.BlankChestnutScroll, "Kruh 5", "Paralyze Scroll");
    public static InscriptionScroll MagicReflection = new InscriptionScroll(Specs.ScrollMagicReflection, Specs.BlankChestnutScroll, "Kruh 5", "Magic Reflection");
    public static InscriptionScroll DispelField = new InscriptionScroll(Specs.ScrollDispelField, Specs.BlankChestnutScroll, "Kruh 5", "Dispel Field Scrol");

    public static InscriptionScroll Reveal = new InscriptionScroll(Specs.ScrollReveal, Specs.BlankOakScroll, "Kruh 6", "Reveal Scroll");
    public static InscriptionScroll Dispel = new InscriptionScroll(Specs.ScrollDispel, Specs.BlankOakScroll, "Kruh 6", "Dispel Scroll");
    public static InscriptionScroll Mark = new InscriptionScroll(Specs.ScrollMark, Specs.BlankOakScroll, "Kruh 6", "Mark Scroll");
    public static InscriptionScroll ParalyzeField = new InscriptionScroll(Specs.ScrollParalyzeField, Specs.BlankOakScroll, "Kruh 6", "Paralyze Field Scr");

    public static InscriptionScroll Flamestrike = new InscriptionScroll(Specs.ScrollFlamestrike, Specs.BlankOakScroll, "Kruh 7", "Flamestrike Scroll");
    public static InscriptionScroll MassDispel = new InscriptionScroll(Specs.ScrollMassDispel, Specs.BlankOakScroll, "Kruh 7", "Mass Dispel Scrol");
    public static InscriptionScroll EnergyFied = new InscriptionScroll(Specs.ScrollEnergyField, Specs.BlankOakScroll, "Kruh 7", "Energy Fied Scroll");
    public static InscriptionScroll GateTravel = new InscriptionScroll(Specs.ScrollGateTravel, Specs.BlankOakScroll, "Kruh 7", "Gate Travel Scroll");

    public static InscriptionScroll Resurrection = new InscriptionScroll(Specs.ScrollResurrection, Specs.BlankScroll, "Kruh 8", "Resurrection Scrol");

    public static InscriptionScroll NecroLight = new InscriptionScroll(Specs.NecroScrollOfLight, Specs.BlankScroll, "Necromancery", "Scroll of Light");
    public static InscriptionScroll NecroBoneArmor = new InscriptionScroll(Specs.NecroScrollOfBoneArmor, Specs.BlankScroll, "Necromancery", "Scroll of Bone Armor");
    public static InscriptionScroll NecroFireBolt = new InscriptionScroll(Specs.NecroScrollOfFireBolt, Specs.BlankScroll, "Necromancery", "Scroll of Fire Bolt");
    public static InscriptionScroll AnimateDead = new InscriptionScroll(Specs.NecroScrollOfAnimateDead, Specs.BlankScroll,
        new CraftResource[] { new CraftResource(Specs.BlankScroll, 1), new CraftResource(Specs.Bone, 3) },
        "Necromancery", "Scroll of Animate Dead");
    public static InscriptionScroll FrostBolt = new InscriptionScroll(Specs.ScrollFrostBolt, Specs.BlankScroll, "Necromancery", "Frost Bolt Scroll");

    public static InscriptionScroll ManaShield = new InscriptionScroll(Specs.ScrollManaShield, Specs.BlankScroll, "Mana Shield Scroll");
}

public static class Inscription
{
    public static ushort BatchSize { get; set; } = 75;
    public static Action OnStart { get; set; }

    public static void Inscribe(InscriptionScroll scroll)
    {
        var producer = new CraftProducer(scroll);
        producer.OnStart = OnStart;
        producer.AdditionalCycleEndPhrases = new[] { "Nemas dost many.", "Nemas dost svitku." };
        producer.BatchSize = BatchSize;
        producer.StartCycle = () =>
        {
            Meditation.Meditate();
            UO.Use(scroll.ScrollSpec);
        };

        producer.Produce();
    }
}

public sealed class InscriptionScroll : CraftProduct
{
    public ItemSpec ScrollSpec { get; }

    public InscriptionScroll(ItemSpec spec, ItemSpec scrollSpec, params string[] path)
        : this(spec, scrollSpec, new CraftResource[] { new CraftResource(scrollSpec, 1) }, path)
    {
    }

    public InscriptionScroll(ItemSpec spec, ItemSpec scrollSpec, CraftResource[] resources, params string[] path)
        : base(spec, resources, path)
    {
        ScrollSpec = scrollSpec;
    }
}

UO.RegisterCommand("inscription-1-weaken", () => Inscription.Inscribe(InscriptionMenu.Weaken));
UO.RegisterCommand("inscription-1-night-sight", () => Inscription.Inscribe(InscriptionMenu.NightSight));
UO.RegisterCommand("inscription-1-magic-arrow", () => Inscription.Inscribe(InscriptionMenu.MagicArrow));
UO.RegisterCommand("inscription-1-feeblemind", () => Inscription.Inscribe(InscriptionMenu.Feeblemind));
UO.RegisterCommand("inscription-1-clumsy", () => Inscription.Inscribe(InscriptionMenu.Clumsy));

UO.RegisterCommand("inscription-2-strength", () => Inscription.Inscribe(InscriptionMenu.Strength));
UO.RegisterCommand("inscription-2-harm", () => Inscription.Inscribe(InscriptionMenu.Harm));
UO.RegisterCommand("inscription-2-cure", () => Inscription.Inscribe(InscriptionMenu.Cure));
UO.RegisterCommand("inscription-2-cunning", () => Inscription.Inscribe(InscriptionMenu.Cunnig));
UO.RegisterCommand("inscription-2-agility", () => Inscription.Inscribe(InscriptionMenu.Agility));

UO.RegisterCommand("inscription-3-wallofstone", () => Inscription.Inscribe(InscriptionMenu.WallOfStone));
UO.RegisterCommand("inscription-3-poison", () => Inscription.Inscribe(InscriptionMenu.Poison));
UO.RegisterCommand("inscription-3-fireball", () => Inscription.Inscribe(InscriptionMenu.Fireball));

UO.RegisterCommand("inscription-4-curse", () => Inscription.Inscribe(InscriptionMenu.Curse));
UO.RegisterCommand("inscription-4-manadrain", () => Inscription.Inscribe(InscriptionMenu.ManaDrain));
UO.RegisterCommand("inscription-4-recall", () => Inscription.Inscribe(InscriptionMenu.Recall));

UO.RegisterCommand("inscription-5-summon-creature", () => Inscription.Inscribe(InscriptionMenu.SummonCreature));
UO.RegisterCommand("inscription-5-paralyze", () => Inscription.Inscribe(InscriptionMenu.Paralyze));
UO.RegisterCommand("inscription-5-magic-reflection", () => Inscription.Inscribe(InscriptionMenu.MagicReflection));
UO.RegisterCommand("inscription-5-dispel-field", () => Inscription.Inscribe(InscriptionMenu.DispelField));

UO.RegisterCommand("inscription-necro-frost-bolt", () => Inscription.Inscribe(InscriptionMenu.FrostBolt));
UO.RegisterCommand("inscription-necro-light", () => Inscription.Inscribe(InscriptionMenu.NecroLight));
UO.RegisterCommand("inscription-necro-bone-armor", () => Inscription.Inscribe(InscriptionMenu.NecroBoneArmor));
UO.RegisterCommand("inscription-necro-fire-bolt", () => Inscription.Inscribe(InscriptionMenu.NecroFireBolt));
UO.RegisterCommand("inscription-necro-animate-dead", () => Inscription.Inscribe(InscriptionMenu.AnimateDead));

UO.RegisterCommand("inscription-gate", () => Inscription.Inscribe(InscriptionMenu.GateTravel));
UO.RegisterCommand("inscription-recall", () => Inscription.Inscribe(InscriptionMenu.Recall));
UO.RegisterCommand("inscription-mark", () => Inscription.Inscribe(InscriptionMenu.Mark));
UO.RegisterCommand("inscription-teleport", () => Inscription.Inscribe(InscriptionMenu.Teleport));
