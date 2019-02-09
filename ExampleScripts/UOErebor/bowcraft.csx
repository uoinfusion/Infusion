#load "common.csx"
#load "items.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class BowcraftMenu
{    
    public static readonly CraftProduct SpruceShaft = new CraftProduct(Specs.SpruceShaft, new CraftResource(Specs.SpruceLog, 1), "Spruce", "spruce shaft");
    public static readonly CraftProduct SpruceShaft2 = new CraftProduct(Specs.SpruceShaft, new CraftResource(Specs.SpruceLog, 1), "Spruce", "2 spruce shafts");
    public static readonly CraftProduct ChestnutShaft = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "chestnut shaft");
    public static readonly CraftProduct ChestnutShaft2 = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "2 chestnut shafts");
}

public static class Bowcraft
{
    public static ushort BatchSize { get; set; } = 100;

    public static void Produce(CraftProduct product)
    {
        var knife = CraftProducer.AskForItem(Specs.Knives, "Select knife");
        
        if (knife == null)
            UO.ClientPrint("Canceling bowcraft");
            
        UO.Use(knife);

        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.StartCycle = () =>
        {
            var startingResource = CraftProducer.AskForItem(Specs.Shaft, "Select shaft you start production with");
            if (startingResource == null)
                throw new InvalidOperationException("Canceling bowcraft");
            UO.Use(startingResource);
        };
        
        producer.Produce();
    }
}

UO.RegisterCommand("bowcraft-chestnutshaft", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft));
UO.RegisterCommand("bowcraft-chestnutshaft2", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft2));
UO.RegisterCommand("bowcraft-spruceshaft", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft));
UO.RegisterCommand("bowcraft-spruceshaft2", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft2));

