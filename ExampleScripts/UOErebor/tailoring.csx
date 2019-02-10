#load "items.csx"
#load "Specs.csx"
#load "warehouse.csx"
#load "afk.csx"
#load "light.csx"
#load "CraftMenu.csx"

using System;
using System.Linq;
using System.Collections.Generic;

public static class TailoringMenu
{
    public static readonly CraftProduct Bag = new CraftProduct(Specs.Bag,
        new CraftResource(Specs.CutUpLeather, 4), "Kozene veci", "Bags", "bag");
    public static readonly CraftProduct Backpack = new CraftProduct(Specs.BackPack,
        new CraftResource(Specs.CutUpLeather, 4), "Kozene veci", "Bags", "backpack");
    public static readonly CraftProduct BeltPouch = new CraftProduct(Specs.BeltPouch,
        new CraftResource(Specs.CutUpLeather, 4), "Kozene veci", "Bags", "belt pouch");
}

public static class Tailoring
{
    public static int TailoringBatchSize = 100;

    public static void Produce(CraftProduct product)
    {
        var sewingKit = CraftProducer.AskForItem(Specs.SewingKit, "Select sewing kit.");
        
        if (sewingKit == null)
            UO.ClientPrint("Canceling tailoring");
            
        var producer = new CraftProducer(product);
        producer.BatchSize = TailoringBatchSize;
        producer.StartCycle = () =>
        {
            var resource = UO.Items.InBackPack()
                .Matching(product.Resources.First().Spec)
                .First();
            UO.WaitTargetObject(resource);
            UO.Use(sewingKit);
        };
        
        producer.Produce();
    }

    public static void Spinning(ItemSpec resourceSpec, ItemSpec productSpec)
        => Processing(resourceSpec, productSpec, Specs.SpinningWheel);
    public static void Weaving(ItemSpec resourceSpec)
        => Processing(resourceSpec, Specs.FoldedCloth, Specs.Loom);
        
    public static void Cut(ItemSpec resourceSpec, ItemSpec productSpec)
    {
        var container = Common.AskForContainer($"Select container with {Specs.TranslateToName(resourceSpec)}");
        if (container == null)
            throw new InvalidOperationException("Cutting canceled.");

        var scissors = UO.Items.InBackPack(true).Matching(Specs.Scissors).FirstOrDefault();
        if (scissors == null)
            throw new InvalidOperationException("Cutting canceled.");

        var productContainer = Warehouse.Global.AskForContainer(productSpec, $"Select container to store {Specs.CutUpLeather}");
        if (productContainer == null)
            throw new InvalidOperationException("Cutting canceled.");
        
        var journal = UO.CreateEventJournal();        
        
        IEnumerable<Item> resources;
        while ((resources = UO.Items.InContainer(container).Matching(resourceSpec)).Any())
        {
            UO.Log($"Remaining {resources.Count()} {Specs.TranslateToName(resourceSpec)}");
            
            var resource = resources.First();
            UO.WaitTargetObject(resource);
            UO.Use(scissors);
            UO.Wait(1000);
            Items.MoveItems(UO.Items.InBackPack().Matching(productSpec), productContainer);
        }
    }

    public static void Processing(ItemSpec resourceSpec, ItemSpec productSpec, ItemSpec instrumentSpec)
    {     
        var instrument = UO.Items.Matching(instrumentSpec).OnGround().MaxDistance(4)
            .OrderByDistance().FirstOrDefault();
        if (instrument == null)
        {
            UO.ClientPrint($"Cannot find any {Specs.TranslateToName(instrumentSpec)}.");
            return;
        }
     
        var resourceContainer = Warehouse.Global
            .AskForContainer(resourceSpec, $"Select container with {Specs.TranslateToName(resourceSpec)}");
        if (resourceContainer == null)
            throw new InvalidOperationException("Tailoring canceled.");
            
        var productContainer = Warehouse.Global
            .AskForContainer(productSpec, $"Select container with {Specs.TranslateToName(productSpec)}");
        if (productContainer == null)
            throw new InvalidOperationException("Tailoring canceled.");
        
        while (true)
        {
            Afk.Check();
            Light.Check();
        
            Items.MoveItems(UO.Items.InBackPack().Matching(productSpec), productContainer);
            Items.Reload(resourceContainer, 1, resourceSpec);
    
            var resource = UO.Items.Matching(resourceSpec).InBackPack().FirstOrDefault();
            
            if (resource != null)
            {
                UO.WaitTargetObject(instrument);
                UO.Use(resource);
                UO.Journal.WaitAny("in your pack.",
                    "at your feet. It is too heavy.");
                UO.Wait(200);
            }
            else
            {
                UO.ClientPrint($"Cannot find {Specs.TranslateToName(resourceSpec)}");
                break;
            }
        }
    }
}

UO.RegisterCommand("tailoring-cotton", () => Tailoring.Spinning(Specs.BaleOfCotton, Specs.SpoolsOfThread));
UO.RegisterCommand("tailoring-wool", () => Tailoring.Spinning(Specs.PilesOfWool, Specs.BallsOfYarn));
UO.RegisterCommand("tailoring-spool", () => Tailoring.Weaving(Specs.SpoolsOfThread));
UO.RegisterCommand("tailoring-yarn", () => Tailoring.Weaving(Specs.BallsOfYarn));
UO.RegisterCommand("tailoring-cut-leather", () => Tailoring.Cut(Specs.PileOfHides, Specs.CutUpLeather));

UO.RegisterCommand("tailoring-backpack", () => Tailoring.Produce(TailoringMenu.Backpack));
UO.RegisterCommand("tailoring-bag", () => Tailoring.Produce(TailoringMenu.Bag));
