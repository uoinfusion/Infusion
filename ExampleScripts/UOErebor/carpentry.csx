#load "common.csx"
#load "items.csx"
#load "CraftMenu.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion;
using Infusion.LegacyApi.Events;

public static class CarpentryMenu
{
    public static readonly CraftProduct Torch = new CraftProduct(Specs.Torch, new CraftResource(Specs.SpruceLog, 1), "Miscellaneous", "torch");
    public static readonly CraftProduct Paper = new CraftProduct(Specs.Paper, new CraftResource(Specs.SpruceLog, 1), "Miscellaneous", "Paper");
    public static readonly CraftProduct BlankScroll = new CraftProduct(Specs.BlankScroll, new CraftResource(Specs.SpruceLog, 1), "Miscellaneous", "blank scroll");
    public static readonly CraftProduct BlankChestnutScroll = new CraftProduct(Specs.BlankChestnutScroll, new CraftResource(Specs.ChestnutLog, 1), "Miscellaneous", "blank chestnut scroll");
    public static readonly CraftProduct BlankOakScroll = new CraftProduct(Specs.BlankOakScroll, new CraftResource(Specs.OakLog, 2), "Miscellaneous", "blank oak scroll");
    public static readonly CraftProduct BlankTeakScroll = new CraftProduct(Specs.BlankTeakScroll, new CraftResource(Specs.TeakLog, 2), "Miscellaneous", "blank teak scroll");
    public static readonly CraftProduct BlankMap = new CraftProduct(Specs.BlankMap, new CraftResource(Specs.ChestnutLog, 1), "Resources & Tools", "Blank Map");
}

public static class Carpentry
{
    public static ushort BatchSize { get; set; } = 75;
    public static Action OnStart { get; set; }

    public static void Produce(CraftProduct product)
    {
        var saw = CraftProducer.AskForItem(Specs.Saw);
        
        var producer = new CraftProducer(product);
        producer.BatchSize = BatchSize;
        producer.OnStart = OnStart;
        producer.StartCycle = () => UO.Use(saw);
        
        producer.Produce();
    }  
}

UO.RegisterCommand("carpentry-torch", () => Carpentry.Produce(CarpentryMenu.Torch));
UO.RegisterCommand("carpentry-paper", () => Carpentry.Produce(CarpentryMenu.Paper));
UO.RegisterCommand("carpentry-blankscroll", () => Carpentry.Produce(CarpentryMenu.BlankScroll));
UO.RegisterCommand("carpentry-blankscroll-chestnut", () => Carpentry.Produce(CarpentryMenu.BlankChestnutScroll));
UO.RegisterCommand("carpentry-blankscroll-oak", () => Carpentry.Produce(CarpentryMenu.BlankOakScroll));
UO.RegisterCommand("carpentry-blankscroll-teak", () => Carpentry.Produce(CarpentryMenu.BlankTeakScroll));
UO.RegisterCommand("carpentry-blankmap", () => Carpentry.Produce(CarpentryMenu.BlankMap));
