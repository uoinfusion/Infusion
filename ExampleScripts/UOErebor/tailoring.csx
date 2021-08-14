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
        
    public static readonly CraftProduct BoneHelmet = new CraftProduct(Specs.BoneHelmet,
        new[] { new CraftResource(Specs.CutUpLeather, 2), new CraftResource(Specs.Bone, 4) },
        "Kozene veci", "Bone Armour", "bone helmet"
    );
    public static readonly CraftProduct BoneArmor = new CraftProduct(Specs.BoneArmor,
        new[] { new CraftResource(Specs.CutUpLeather, 5), new CraftResource(Specs.Bone, 10) },
        "Kozene veci", "Bone Armour", "bone armor"
    );
    public static readonly CraftProduct BoneArms = new CraftProduct(Specs.BoneArms,
        new[] { new CraftResource(Specs.CutUpLeather, 1), new CraftResource(Specs.Bone, 2) },
        "Kozene veci", "Bone Armour", "bone arms"
    );
    public static readonly CraftProduct BoneLeggings = new CraftProduct(Specs.BoneLeggings,
        new[] { new CraftResource(Specs.CutUpLeather, 3), new CraftResource(Specs.Bone, 6) },
        "Kozene veci", "Bone Armour", "bone leggings"
    );
    public static readonly CraftProduct BoneGloves = new CraftProduct(Specs.BoneLeggings,
        new[] { new CraftResource(Specs.CutUpLeather, 2), new CraftResource(Specs.Bone, 3) },
        "Kozene veci", "Bone Armour", "bone gloves"
    );

    public static readonly CraftProduct Shirt = new CraftProduct(Specs.Shirt,
        new[] { new CraftResource(Specs.FoldedCloth, 8), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "shirt"
    );
    public static readonly CraftProduct FancyShirt = new CraftProduct(Specs.FancyShirt,
        new[] { new CraftResource(Specs.FoldedCloth, 8), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "fancy shirt"
    );
    public static readonly CraftProduct PlainDress = new CraftProduct(Specs.PlainDress,
        new[] { new CraftResource(Specs.FoldedCloth, 10), new CraftResource(Specs.SpoolsOfThread, 4) },
        "Platene veci", "Shirts", "plain dress"
    );
    public static readonly CraftProduct FancyDress = new CraftProduct(Specs.FancyDress,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 4) },
        "Platene veci", "Shirts", "fancy dress"
    );
    public static readonly CraftProduct JesterSuit = new CraftProduct(Specs.FancyDress,
        new[] { new CraftResource(Specs.FoldedCloth, 16), new CraftResource(Specs.SpoolsOfThread, 6) },
        "Platene veci", "Shirts", "jester suit"
    );
    public static readonly CraftProduct Cloak = new CraftProduct(Specs.Cloak,
        new[] { new CraftResource(Specs.FoldedCloth, 14), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "cloak"
    );
    public static readonly CraftProduct Tunic = new CraftProduct(Specs.Tunic,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "tunic"
    );
    public static readonly CraftProduct Surcoat = new CraftProduct(Specs.Surcoat,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "surcoat"
    );
    public static readonly CraftProduct Doublet = new CraftProduct(Specs.Doublet,
        new[] { new CraftResource(Specs.FoldedCloth, 8), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Shirts", "doublet"
    );
    
    public static readonly CraftProduct ShortPants = new CraftProduct(Specs.ShortPants,
        new[] { new CraftResource(Specs.FoldedCloth, 6), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Pants", "short pants"
    );
    public static readonly CraftProduct LongPants = new CraftProduct(Specs.LongPants,
        new[] { new CraftResource(Specs.FoldedCloth, 8), new CraftResource(Specs.SpoolsOfThread, 4) },
        "Platene veci", "Pants", "long pants"
    );
    public static readonly CraftProduct Skirt = new CraftProduct(Specs.Skirt,
        new[] { new CraftResource(Specs.FoldedCloth, 10), new CraftResource(Specs.SpoolsOfThread, 4) },
        "Platene veci", "Pants", "skirt"
    );
    public static readonly CraftProduct Kilt = new CraftProduct(Specs.Kilt,
        new[] { new CraftResource(Specs.FoldedCloth, 8), new CraftResource(Specs.SpoolsOfThread, 3) },
        "Platene veci", "Pants", "kilt"
    );

    public static readonly CraftProduct Bandana = new CraftProduct(Specs.Bandana,
        new[] { new CraftResource(Specs.FoldedCloth, 1), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "bandana"
    );
    public static readonly CraftProduct Skullcap = new CraftProduct(Specs.Skullcap,
        new[] { new CraftResource(Specs.FoldedCloth, 2), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "skullcap"
    );
    public static readonly CraftProduct FloppyHat = new CraftProduct(Specs.FloppyHat,
        new[] { new CraftResource(Specs.FoldedCloth, 11), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "floppy hat"
    );
    public static readonly CraftProduct WideBrimHat = new CraftProduct(Specs.WideBrimHat,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "wide-brim hat"
    );
    public static readonly CraftProduct TricorneHat = new CraftProduct(Specs.TricorneHat,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 2) },
        "Platene veci", "Headwear", "tricorne hat"
    );
    public static readonly CraftProduct TallStrawHat = new CraftProduct(Specs.TallStrawHat,
        new[] { new CraftResource(Specs.FoldedCloth, 12), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "tall straw hat"
    );
    public static readonly CraftProduct StrawHat = new CraftProduct(Specs.StrawHat,
        new[] { new CraftResource(Specs.FoldedCloth, 10), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "straw hat"
    );
    public static readonly CraftProduct Bonnet = new CraftProduct(Specs.Bonnet,
        new[] { new CraftResource(Specs.FoldedCloth, 11), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "bonnet"
    );
    public static readonly CraftProduct Cap = new CraftProduct(Specs.Cap,
        new[] { new CraftResource(Specs.FoldedCloth, 5), new CraftResource(Specs.SpoolsOfThread, 1) },
        "Platene veci", "Headwear", "cap"
    );

    public static readonly CraftProduct BoltOfCloth = new CraftProduct(Specs.BoltOfCloth,
        new[] { new CraftResource(Specs.FoldedCloth, 50) },
        "Platene veci", "bolt of cloth"
    );

    public static readonly ItemSpec AllSewingProducts = new[] {
        Specs.Shirt, Specs.FancyShirt, Specs.Cloak, Specs.Tunic, Specs.Surcoat, Specs.PlainDress, Specs.FancyDress, Specs.JesterSuit, Specs.Doublet,
        Specs.ShortPants, Specs.LongPants, Specs.Skirt, Specs.Kilt,
        Specs.Bandana, Specs.Skullcap, Specs.FloppyHat, Specs.WideBrimHat, Specs.TricorneHat, Specs.TallStrawHat, Specs.StrawHat, Specs.Bonnet, Specs.Cap
    };
}

public static class Tailoring
{
    public static int TailoringBatchSize = 100;

    public static void Produce(CraftProduct product)
    {
        Produce(product, () => {}, TailoringBatchSize);
    }

    public static void Produce(CraftProduct product, Action postproduce, int batchSize)
    {
        var sewingKit = CraftProducer.AskForItem(Specs.SewingKit, "Select sewing kit.");
        
        if (sewingKit == null)
            UO.ClientPrint("Canceling tailoring");
            
        var producer = new CraftProducer(product);
        producer.BatchSize = batchSize;
        producer.StartCycle = () =>
        {
            var resource = UO.Items.InBackPack()
                .Matching(product.Resources.First().Spec)
                .First();
            UO.WaitTargetObject(resource);
            UO.Use(sewingKit);
        };
        producer.Postproduce = postproduce;
        
        producer.Produce();
    }

    public static void TrainSewing(CraftProduct product, int batchSize)
    {
        var trainingProduct = new CraftProduct(Specs.Bandage, product.Resources, product.Path);
        Action postproduce = () =>
        {
            var intermediateItems = UO.Items.InBackPack().Matching(TailoringMenu.AllSewingProducts);
            foreach (var item in intermediateItems)
            {
                UO.Use(Specs.Scissors);
                UO.WaitForTarget();
                UO.Target(item);
            }
            UO.Wait(1000);
        };
        Produce(trainingProduct, postproduce, batchSize);
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

UO.RegisterCommand("tailoring-shirts-cloak", () => Tailoring.TrainSewing(TailoringMenu.Cloak, 25));
UO.RegisterCommand("tailoring-shirts-fancyshirt", () => Tailoring.TrainSewing(TailoringMenu.FancyShirt, 25));
UO.RegisterCommand("tailoring-shirts-shirt", () => Tailoring.TrainSewing(TailoringMenu.Shirt, 25));
UO.RegisterCommand("tailoring-shirts-surcoat", () => Tailoring.TrainSewing(TailoringMenu.Surcoat, 25));
UO.RegisterCommand("tailoring-shirts-tunic", () => Tailoring.TrainSewing(TailoringMenu.Tunic, 25));
UO.RegisterCommand("tailoring-shirts-plaindress", () => Tailoring.TrainSewing(TailoringMenu.PlainDress, 30));
UO.RegisterCommand("tailoring-shirts-fancydress", () => Tailoring.TrainSewing(TailoringMenu.FancyDress, 30));
UO.RegisterCommand("tailoring-shirts-jestersuit", () => Tailoring.TrainSewing(TailoringMenu.JesterSuit, 25));
UO.RegisterCommand("tailoring-shirts-doublet", () => Tailoring.TrainSewing(TailoringMenu.Doublet, 30));

UO.RegisterCommand("tailoring-pants-shortpants", () => Tailoring.TrainSewing(TailoringMenu.ShortPants, 30));
UO.RegisterCommand("tailoring-pants-longpants", () => Tailoring.TrainSewing(TailoringMenu.LongPants, 30));
UO.RegisterCommand("tailoring-pants-skirt", () => Tailoring.TrainSewing(TailoringMenu.Skirt, 30));
UO.RegisterCommand("tailoring-pants-kilt", () => Tailoring.TrainSewing(TailoringMenu.Kilt, 30));

UO.RegisterCommand("tailoring-headwear-bandana", () => Tailoring.TrainSewing(TailoringMenu.Bandana, 100));
UO.RegisterCommand("tailoring-headwear-skullcap", () => Tailoring.TrainSewing(TailoringMenu.Skullcap, 100));
UO.RegisterCommand("tailoring-headwear-floppyhat", () => Tailoring.TrainSewing(TailoringMenu.FloppyHat, 40));
UO.RegisterCommand("tailoring-headwear-widebrimhat", () => Tailoring.TrainSewing(TailoringMenu.WideBrimHat, 40));
UO.RegisterCommand("tailoring-headwear-tricornhat", () => Tailoring.TrainSewing(TailoringMenu.TricorneHat, 40));
UO.RegisterCommand("tailoring-headwear-tallstrawhat", () => Tailoring.TrainSewing(TailoringMenu.TallStrawHat, 40));
UO.RegisterCommand("tailoring-headwear-strawhat", () => Tailoring.TrainSewing(TailoringMenu.StrawHat, 40));
UO.RegisterCommand("tailoring-headwear-bonnet", () => Tailoring.TrainSewing(TailoringMenu.Bonnet, 40));
UO.RegisterCommand("tailoring-headwear-cap", () => Tailoring.TrainSewing(TailoringMenu.Cap, 75));

UO.RegisterCommand("tailoring-bolt-of-cloth", () => Tailoring.TrainSewing(TailoringMenu.BoltOfCloth, 5));

UO.RegisterCommand("tailoring-backpack", () => Tailoring.Produce(TailoringMenu.Backpack));
UO.RegisterCommand("tailoring-bag", () => Tailoring.Produce(TailoringMenu.Bag));

UO.RegisterCommand("tailoring-bone-armor", () => Tailoring.Produce(TailoringMenu.BoneArmor));
UO.RegisterCommand("tailoring-bone-leggings", () => Tailoring.Produce(TailoringMenu.BoneLeggings));
UO.RegisterCommand("tailoring-bone-arms", () => Tailoring.Produce(TailoringMenu.BoneArms));
UO.RegisterCommand("tailoring-bone-gloves", () => Tailoring.Produce(TailoringMenu.BoneGloves));
UO.RegisterCommand("tailoring-bone-helmet", () => Tailoring.Produce(TailoringMenu.BoneHelmet));
