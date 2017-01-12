using System;
using System.IO;
using System.Linq;
using UltimaRX.Packets;
using static UltimaRX.Proxy.InjectionApi.Injection;

public static class Scripts
{
    public static string[] skipOre = {"Copper Ore"};

    public static void HarvestTrees(string mapFileInfoFile)
    {
        Harvest(mapFileInfoFile, HarvestTree);
    }

    public static void HarvestOre(string mapFileInfoFile)
    {
        Harvest(mapFileInfoFile, Mine);
    }

    public static void Harvest(string mapFileInfoFile, Action<string> harvestAction)
    {
        var mapLines = File.ReadAllLines(mapFileInfoFile);
        foreach (var mapLine in mapLines)
        {
            ProcessHarvestMapLine(mapLine, harvestAction);
        }
    }

    public static void ProcessHarvestMapLine(string mapLine, Action<string> harvestAction)
    {
        if (mapLine.StartsWith("harvest: "))
        {
            var parameters = mapLine.Substring("harvest: ".Length);
            harvestAction(parameters);
        }
        else if (mapLine.StartsWith("walk: "))
        {
            var parameters = mapLine.Substring("walk: ".Length).Split(',').Select(x => ushort.Parse(x.Trim())).ToArray();
            Log($"Walking to: {parameters[0]}, {parameters[1]}");
            WalkTo(parameters[0], parameters[1]);
        }
    }

    public static void Mine(string tileInfo)
    {
        Log($"Mining {tileInfo}");
        var canMine = true;

        while (canMine)
        {
            Wait(1000);
            DeleteJournal();
            UseType(ItemTypes.PickAxe);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You put", "Nevykopal jsi nic zajimaveho", "Try mining in rock",
                "There is no ore here to mine");

            canMine = !InJournal("Try mining in rock", "There is no ore here to mine");
            canMine &= !skipOre.Any(s => InJournal(s));

            if (canMine && InJournal("Jeste nemuzes pouzit skill."))
            {
                Log("waiting for skill");
                Wait(5000);
            }
        }
    }

    public static void Fish(string tileInfo)
    {
        Log($"Fishing {tileInfo}");
        var canFish = true;

        while (canFish)
        {
            DeleteJournal();
            UseType(ItemTypes.FishingPole);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You pull out a fish", "There are no fish here", "Dneska nejak neberou");

            Wait(1000);

            var fish = Items.FindTypeOnGround(ItemTypes.Fishes);
            while (fish != null)
            {
                Pickup(fish);
                fish = Items.FindTypeOnGround(ItemTypes.Fishes);
            }

            canFish = !InJournal("There are no fish here");
            if (canFish && InJournal("Jeste nemuzes pouzit skill."))
            {
                Log("waiting for skill");
                Wait(5000);
            }
        }
    }

    public static void HarvestTree(string tileInfo)
    {
        Log($"Lumberjacking {tileInfo}");

        var treeHarvestable = true;

        while (treeHarvestable)
        {
            Wait(1000);
            DeleteJournal();
            UseType(ItemTypes.Hatchets);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs");

            treeHarvestable = !InJournal("of a way to use", "immune", "There are no logs here to chop.");

            if (treeHarvestable && InJournal("Jeste nemuzes pouzit skill."))
            {
                Log("waiting for skill");
                Wait(5000);
            }
        }
    }

    public static void StepToward(Location2D currentLocation, Location2D targetLocation)
    {
        var walkVector = (targetLocation - currentLocation).Normalize();
        if (walkVector != Vector.NullVector)
        {
            WaitWalk();
            Walk(walkVector.ToDirection());
        }
    }

    public static void StepToward(Item item)
    {
        StepToward((Location2D) item.Location);
    }

    public static void StepToward(Location2D targetLocation)
    {
        StepToward((Location2D) Me.PredictedLocation, targetLocation);
    }

    public static void WalkTo(Location2D targetLocation)
    {
        while ((Location2D) Me.PredictedLocation != targetLocation)
        {
            StepToward(targetLocation);
        }
    }

    public static void WalkTo(ushort xloc, ushort yloc)
    {
        WalkTo(new Location2D(xloc, yloc));
    }

    public static void Cook(ModelId rawFoodType)
    {
        var campfire = Items.FindType(ItemTypes.Campfire);
        var rawFood = Items.FindType(rawFoodType);

        while (campfire != null && rawFood != null)
        {
            Use(rawFood);
            WaitForTarget();
            Target(campfire);

            WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
            Wait(500);

            campfire = Items.FindType(ItemTypes.Campfire);
            rawFood = Items.FindType(rawFoodType);
        }
    }

    public static void Cook(ModelId rawFoodType, string campfireTile)
    {
        var rawFood = Items.FindType(rawFoodType);

        while (rawFood != null)
        {
            Use(rawFood);
            WaitForTarget();
            TargetTile(campfireTile);

            WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
            Wait(500);

            rawFood = Items.FindType(rawFoodType);
        }
    }

    public static void Cook()
    {
        var campfireTile = Info();

        var rawFood = Items.FindType(ItemTypes.RawFood);
        while (rawFood != null)
        {
            Cook(rawFood.Type, campfireTile);
            rawFood = Items.FindType(ItemTypes.RawFood);
        }
    }

    public static void Loot()
    {
        var container = ItemInfo();
        if (container != null)
        {
            Loot(container);
        }
        else
            Log("no container for loot");
    }

    public static void Loot(Item container)
    {
        var item = Items.InContainer(container);
        while (item != null)
        {
            Log("looting item");
            Pickup(item);
            item = Items.InContainer(container);
        }

        Log("nothing to loot");
    }

    public static void Kill(Item subjectToKill)
    {
        WarModeOn();
        Attack(subjectToKill);

        while (subjectToKill != null)
        {
            if (subjectToKill.GetDistance(Me.Location) > 1)
                StepToward(subjectToKill);
            subjectToKill = Items.RefreshItem(subjectToKill);
        }

        Item bodyOfSubject;
        var retryCount = 0;

        do
        {
            Log("waiting for body");
            Wait(1000);
            bodyOfSubject = Items.FindTypeAll(ItemTypes.RippableBodies).OrderBy(i => i.GetDistance(Me.Location)).First();
            retryCount++;
        } while (bodyOfSubject == null && retryCount < 20);

        if (bodyOfSubject != null)
        {
            DeleteJournal();
            UseType(ItemTypes.Knives);
            WaitForTarget();
            Target(bodyOfSubject);
            WaitForJournal("Rozrezal jsi mrtvolu.");
            Wait(1000);
            Loot(bodyOfSubject);
        }
        else
            Log("no body found");

        WarModeOff();
    }

    public static void Kill()
    {
        var subjectToKill = ItemInfo();
        if (subjectToKill != null)
            Kill(subjectToKill);
    }

    public static void MassKill()
    {
        do
        {
            var subject =
                Items.FindTypeAll(ItemTypes.MassKillSubjects).OrderBy(i => i.GetDistance(Me.Location)).FirstOrDefault();
            if (subject == null)
            {
                Log("nothing to kill");
                break;
            }

            Kill(subject);
        } while (true);
    }
}