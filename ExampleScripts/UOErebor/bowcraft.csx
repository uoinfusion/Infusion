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
    public static readonly CraftProduct SpruceShaft3 = new CraftProduct(Specs.SpruceShaft, new CraftResource(Specs.SpruceLog, 1), "Spruce", "3 spruce shafts");
    public static readonly CraftProduct SpruceShaft4 = new CraftProduct(Specs.SpruceShaft, new CraftResource(Specs.SpruceLog, 1), "Spruce", "4 spruce shafts");
    public static readonly CraftProduct SpruceShaft5 = new CraftProduct(Specs.SpruceShaft, new CraftResource(Specs.SpruceLog, 1), "Spruce", "5 spruce shafts");
    public static readonly CraftProduct ChestnutShaft = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "chestnut shaft");
    public static readonly CraftProduct ChestnutShaft2 = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "2 chestnut shafts");
    public static readonly CraftProduct ChestnutShaft3 = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "3 chestnut shafts");
    public static readonly CraftProduct ChestnutShaft4 = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "4 chestnut shafts");
    public static readonly CraftProduct ChestnutShaft5 = new CraftProduct(Specs.ChestnutShaft, new CraftResource(Specs.ChestnutLog, 1), "Chestnut", "5 chestnut shafts");
    public static readonly CraftProduct OakShaft = new CraftProduct(Specs.OakShaft, new CraftResource(Specs.OakLog, 1), "Oak", "oak shaft");
    public static readonly CraftProduct OakShaft2 = new CraftProduct(Specs.OakShaft, new CraftResource(Specs.OakLog, 1), "Oak", "2 oak shafts");
    public static readonly CraftProduct OakShaft3 = new CraftProduct(Specs.OakShaft, new CraftResource(Specs.OakLog, 1), "Oak", "3 oak shafts");
    public static readonly CraftProduct OakShaft4 = new CraftProduct(Specs.OakShaft, new CraftResource(Specs.OakLog, 1), "Oak", "4 oak shafts");
    public static readonly CraftProduct OakShaft5 = new CraftProduct(Specs.OakShaft, new CraftResource(Specs.OakLog, 1), "Oak", "5 oak shafts");
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

UO.RegisterCommand("bowcraft-spruce-shaft", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft));
UO.RegisterCommand("bowcraft-spruce-shaft2", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft2));
UO.RegisterCommand("bowcraft-spruce-shaft3", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft3));
UO.RegisterCommand("bowcraft-spruce-shaft4", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft4));
UO.RegisterCommand("bowcraft-spruce-shaft5", () => Bowcraft.Produce(BowcraftMenu.SpruceShaft5));
UO.RegisterCommand("bowcraft-chestnut-shaft", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft));
UO.RegisterCommand("bowcraft-chestnut-shaft2", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft2));
UO.RegisterCommand("bowcraft-chestnut-shaft3", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft3));
UO.RegisterCommand("bowcraft-chestnut-shaft4", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft4));
UO.RegisterCommand("bowcraft-chestnut-shaft5", () => Bowcraft.Produce(BowcraftMenu.ChestnutShaft5));
UO.RegisterCommand("bowcraft-oak-shaft", () => Bowcraft.Produce(BowcraftMenu.OakShaft));
UO.RegisterCommand("bowcraft-oak-shaft2", () => Bowcraft.Produce(BowcraftMenu.OakShaft2));
UO.RegisterCommand("bowcraft-oak-shaft3", () => Bowcraft.Produce(BowcraftMenu.OakShaft3));
UO.RegisterCommand("bowcraft-oak-shaft4", () => Bowcraft.Produce(BowcraftMenu.OakShaft4));
UO.RegisterCommand("bowcraft-oak-shaft5", () => Bowcraft.Produce(BowcraftMenu.OakShaft5));

