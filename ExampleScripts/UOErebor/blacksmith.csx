#load "common.csx"
#load "items.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class BlacksmithMenu
{    
    public static readonly CraftProduct CopperIngot = new CraftProduct(Specs.CopperIngot, new CraftResource(Specs.CopperOre, 1), "Ingots", "Copper ingot");
    public static readonly CraftProduct IronIngot = new CraftProduct(Specs.IronIngot, new CraftResource(Specs.IronOre, 1), "Ingots", "iron ingot");       
    public static readonly CraftProduct VeriteIngot = new CraftProduct(Specs.VeriteIngot, new CraftResource(Specs.VeriteOre, 1), "Ingots", "Verite ingot");       
    public static readonly CraftProduct ValoriteIngot = new CraftProduct(Specs.ValoriteIngot, 
        new[] { new CraftResource(Specs.ValoriteOre, 1), new CraftResource(Specs.Uhli, 1) }, 
        "Ingots", "Valorite ingot");       
}

public static class Blacksmith
{
    public static ushort BatchSize { get; set; } = 100;

    public static void IngotProduce(CraftProduct product)
    {
        var startingResource = CraftProducer.AskForItem(Specs.Ore, "Select ore you start ingot production with", true);
        var hammer = CraftProducer.AskForItem(Specs.SmithsHammer);
        
        if (startingResource == null || hammer == null)
            UO.ClientPrint("Canceling blacksmith");
            
        UO.Use(hammer);

        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.StartCycle = () =>
        {
            UO.Use(startingResource);
        };
        
        producer.Produce();
    }

    public static void Produce(CraftProduct product)
    {
        var hammer = CraftProducer.AskForItem(Specs.SmithsHammer);
        
        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.StartCycle = () => UO.Use(hammer);
        
        producer.Produce();
    }  
}

UO.RegisterCommand("blacksmith-copperingot", () => Blacksmith.IngotProduce(BlacksmithMenu.CopperIngot));
UO.RegisterCommand("blacksmith-ironingot", () => Blacksmith.IngotProduce(BlacksmithMenu.IronIngot));
UO.RegisterCommand("blacksmith-veriteingot", () => Blacksmith.IngotProduce(BlacksmithMenu.VeriteIngot));
UO.RegisterCommand("blacksmith-valoriteingot", () => Blacksmith.IngotProduce(BlacksmithMenu.ValoriteIngot));

