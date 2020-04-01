#load "CraftMenu.csx"

using System;

public static class TinkeringMenu
{
    public static readonly CraftProduct CopperWire = new CraftProduct(Specs.CopperWire, new CraftResource(Specs.CopperIngot, 1), "Parts", "copper wire");
    public static readonly CraftProduct Nails = new CraftProduct(Specs.Nails, new CraftResource(Specs.IronIngot, 1), "Parts", "nails");
    public static readonly CraftProduct Lockpick = new CraftProduct(Specs.Lockpick, new CraftResource(Specs.IronIngot, 1), "Tools", "Lockpick");
    public static readonly CraftProduct EmptyBottle = new CraftProduct(Specs.EmptyBottle, new CraftResource(Specs.Sklovina, 2), "empty bottle");
}

public static class Tinkering
{
    public static ushort BatchSize { get; set; } = 75;
    public static Action OnStart { get; set; }

    public static void Produce(CraftProduct product)
    {
        var tool = CraftProducer.AskForItem(Specs.TinkeringTools);

        var producer = new CraftProducer(product);
        producer.OnStart = OnStart;
        producer.BatchSize = BatchSize;
        producer.StartCycle = () => UO.Use(tool);

        producer.Produce();
    }
}

UO.RegisterCommand("tinkering-wire", () => Tinkering.Produce(TinkeringMenu.CopperWire));
UO.RegisterCommand("tinkering-nails", () => Tinkering.Produce(TinkeringMenu.Nails));
UO.RegisterCommand("tinkering-lockpick", () => Tinkering.Produce(TinkeringMenu.Lockpick));
UO.RegisterCommand("tinkering-bottle", () => Tinkering.Produce(TinkeringMenu.EmptyBottle));