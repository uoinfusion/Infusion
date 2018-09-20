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
    public static readonly CraftProduct CopperIngot = new CraftProduct(Specs.CopperIngot, new CraftResource(Specs.CopperOre, 1), "Copper", "Copper ingot");
       
}

public static class Blacksmith
{
    public static ushort BatchSize { get; set; } = 10;

    public static void Produce(CraftProduct product)
    {
        var hammer = CraftProducer.AskForItem(Specs.SmithsHammer);
        
        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.StartCycle = () => UO.Use(hammer);
        
        producer.Produce();
    }  
}

UO.RegisterCommand("blacksmith-copperingot", () => Blacksmith.Produce(BlacksmithMenu.CopperIngot));

