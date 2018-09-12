#load "common.csx"
#load "items.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class AlchemyMenu
{
    public static readonly CraftProduct Nightsight = new CraftProduct(Specs.NightsighPoition, new[] { new CraftResource(Specs.SpidersSilk, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Nightsight");
    
    public static readonly CraftProduct CureLesser = new CraftProduct(Specs.CureLesserPotion, new[] { new CraftResource(Specs.Garlic, 2), new CraftResource(Specs.EmptyBottle, 1)}, "Cure", "Lesser Cure");
    public static readonly CraftProduct Cure = new CraftProduct(Specs.CurePotion, new[] { new CraftResource(Specs.Garlic, 3), new CraftResource(Specs.Ginseng, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Cure", "Cure");
    
    public static readonly CraftProduct HealLesser = new CraftProduct(Specs.HealLesserPotion, new[] { new CraftResource(Specs.Ginseng, 2), new CraftResource(Specs.Garlic, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Heal", "Lesser Heal");
    public static readonly CraftProduct Heal = new CraftProduct(Specs.HealPotion, new[] { new CraftResource(Specs.Ginseng, 4), new CraftResource(Specs.Garlic, 2), new CraftResource(Specs.EmptyBottle, 1)}, "Heal", "Heal");
    
    public static readonly CraftProduct RefreshLesser = new CraftProduct(Specs.RefreshLesserPotion, new[] { new CraftResource(Specs.BlackPearl, 1), new CraftResource(Specs.SpidersSilk, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Refresh", "Lesser Refresh");
    public static readonly CraftProduct Refresh = new CraftProduct(Specs.RefreshPotion, new[] { new CraftResource(Specs.BlackPearl, 2), new CraftResource(Specs.SpidersSilk, 2), new CraftResource(Specs.EmptyBottle, 1)}, "Refresh", "Refresh");
    
    public static readonly CraftProduct StrengthLesser = new CraftProduct(Specs.StrengthLesserPotion, new[] { new CraftResource(Specs.FertileDirt, 2), new CraftResource(Specs.Mandrake, 1), new CraftResource(Specs.WyrmsHeart, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Strength", "Lesser Strength");
    public static readonly CraftProduct Strength = new CraftProduct(Specs.StrengthPotion, new[] { new CraftResource(Specs.FertileDirt, 3), new CraftResource(Specs.Mandrake, 2), new CraftResource(Specs.WyrmsHeart, 2), new CraftResource(Specs.EmptyBottle, 1)}, "Strength", "Strength");
    public static readonly CraftProduct StrengthGreater = new CraftProduct(Specs.StrengthGreaterPotion, new[] { new CraftResource(Specs.FertileDirt, 4), new CraftResource(Specs.Mandrake, 3), new CraftResource(Specs.WyrmsHeart, 3), new CraftResource(Specs.EmptyBottle, 1)}, "Strength", "Greater Strength");
    
    public static readonly CraftProduct ClevernessLesser = new CraftProduct(Specs.ClevernessLesserPotion, new[] { new CraftResource(Specs.FertileDirt, 1), new CraftResource(Specs.Obsidian, 1), new CraftResource(Specs.EyeOfNewt, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Cleverness", "Lesser Cleverness");
    public static readonly CraftProduct Cleverness = new CraftProduct(Specs.ClevernessPotion, new[] { new CraftResource(Specs.FertileDirt, 3), new CraftResource(Specs.Obsidian, 2), new CraftResource(Specs.EyeOfNewt, 2), new CraftResource(Specs.EmptyBottle, 1)}, "Cleverness", "Cleverness");
    
    public static readonly CraftProduct EnergyLesser = new CraftProduct(Specs.EnergyLesserPotion, new[] { new CraftResource(Specs.BatWings, 2), new CraftResource(Specs.BlackPearl, 2), new CraftResource(Specs.PigIron, 1), new CraftResource(Specs.EmptyBottle, 1)}, "Energy", "Lesser Energy");
    public static readonly CraftProduct Energy = new CraftProduct(Specs.EnergyPotion, new[] { new CraftResource(Specs.BatWings, 2), new CraftResource(Specs.BlackPearl, 4), new CraftResource(Specs.PigIron, 4), new CraftResource(Specs.EmptyBottle, 1)}, "Energy", "Energy");
}

public static class Alchemy
{
    public static ushort BatchSize { get; set; } = 10;

    public static void Produce(CraftProduct product)
    {
        var mortar = CraftProducer.AskForItem(Specs.Mortar);
        
        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.StartCycle = () => UO.Use(mortar);
        
        producer.Produce();
    }  
}

UO.RegisterCommand("alchemy-nightsight", () => Alchemy.Produce(AlchemyMenu.Nightsight));

UO.RegisterCommand("alchemy-curelesser", () => Alchemy.Produce(AlchemyMenu.CureLesser));
UO.RegisterCommand("alchemy-cure", () => Alchemy.Produce(AlchemyMenu.Cure));

UO.RegisterCommand("alchemy-heallesser", () => Alchemy.Produce(AlchemyMenu.HealLesser));
UO.RegisterCommand("alchemy-heal", () => Alchemy.Produce(AlchemyMenu.Heal));

UO.RegisterCommand("alchemy-refreshlesser", () => Alchemy.Produce(AlchemyMenu.RefreshLesser));
UO.RegisterCommand("alchemy-refresh", () => Alchemy.Produce(AlchemyMenu.Refresh));

UO.RegisterCommand("alchemy-strengthlesser", () => Alchemy.Produce(AlchemyMenu.StrengthLesser));
UO.RegisterCommand("alchemy-strength", () => Alchemy.Produce(AlchemyMenu.Strength));
UO.RegisterCommand("alchemy-strengthgreater", () => Alchemy.Produce(AlchemyMenu.StrengthGreater));

UO.RegisterCommand("alchemy-clevernesslesser", () => Alchemy.Produce(AlchemyMenu.ClevernessLesser));
UO.RegisterCommand("alchemy-cleverness", () => Alchemy.Produce(AlchemyMenu.Cleverness));

UO.RegisterCommand("alchemy-energylesser", () => Alchemy.Produce(AlchemyMenu.EnergyLesser));
UO.RegisterCommand("alchemy-energy", () => Alchemy.Produce(AlchemyMenu.Energy));