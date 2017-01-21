using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UltimaRX.Gumps;
using UltimaRX.Packets;
using UltimaRX.Proxy;
using UltimaRX.Proxy.InjectionApi;
using static UltimaRX.Proxy.InjectionApi.Injection;

public static class Scripts
{
    public static string[] skipOre = {"Copper Ore"};

    public static DateTime lastFailedLumberjackingAttempt;

    public static void Harvest(string mapFileInfoFile)
    {
        Log($"Map: {mapFileInfoFile}");

        var mapLines = File.ReadAllLines(mapFileInfoFile);
        var line = 1;
        foreach (var mapLine in mapLines)
        {
            Log($"Processing line {line}");
            ProcessHarvestMapLine(mapLine, null);
            line++;
        }
    }

    public static void ProcessHarvestMapLine(string mapLine, Action<string> harvestAction)
    {
        if (mapLine.StartsWith("harvest: "))
        {
            var parameters = mapLine.Substring("harvest: ".Length);
            harvestAction?.Invoke(parameters);
        }
        if (mapLine.StartsWith("lumber:"))
        {
            var parameters = mapLine.Substring("lumber: ".Length);
            HarvestTree(parameters);
        }
        else if (mapLine.StartsWith("walk: "))
        {
            var parameters = mapLine.Substring("walk: ".Length).Split(',').Select(x => ushort.Parse(x.Trim())).ToArray();
            Log($"Walking to: {parameters[0]}, {parameters[1]}");
            WalkTo(parameters[0], parameters[1]);
        }
        else if (mapLine.StartsWith("masskill"))
        {
            MassKill();
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

            var fishesOnGround = Items.OfType(ItemTypes.Fishes).OnGround();
            foreach (var fish in fishesOnGround)
                Pickup(fish);

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

        var sinceLastFail = DateTime.UtcNow - lastFailedLumberjackingAttempt;
        Log($"Since last lumberjacking fail: {sinceLastFail}");
        if (sinceLastFail < TimeSpan.FromSeconds(6))
        {
            var waitSpan = TimeSpan.FromSeconds(6) - sinceLastFail;
            Log($"Waiting due to lumberjacking fail: {waitSpan}");
            Wait(waitSpan);
        }

        while (treeHarvestable)
        {
            Wait(1000);
            DeleteJournal();
            UseType(ItemTypes.Hatchets);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs",
                "You cannot reach that");

            treeHarvestable =
                !InJournal("of a way to use", "immune", "There are no logs here to chop.", "You cannot reach that");
            if (!treeHarvestable)
            {
                lastFailedLumberjackingAttempt = DateTime.UtcNow;
                Log($"Last lumberjacking fail: {lastFailedLumberjackingAttempt:T}");
            }
            else if (InJournal("Jeste nemuzes pouzit skill."))
            {
                Log("waiting for skill");
                Wait(5000);
            }
        }
    }

    public static void StepToward(Location2D currentLocation, Location2D targetLocation)
    {
        Program.Diagnostic.WriteLine($"StepToward: {currentLocation} -> {targetLocation}");
        var walkVector = (targetLocation - currentLocation).Normalize();
        if (walkVector != Vector.NullVector)
        {
            Program.Diagnostic.WriteLine($"StepToward: walkVector = {walkVector}");
            WaitToAvoidFastWalk();
            Walk(walkVector.ToDirection(), MovementType.Run);
            WaitWalkAcknowledged();
        }
        else
            Program.Diagnostic.WriteLine("walkVector is Vector.NullVector");
    }

    public static void StepToward(Item item)
    {
        StepToward((Location2D) item.Location);
    }

    public static void StepToward(Location2D targetLocation)
    {
        StepToward((Location2D) Me.Location, targetLocation);
    }

    public static void WalkTo(Location2D targetLocation)
    {
        while ((Location2D) Me.Location != targetLocation)
        {
            Program.Diagnostic.WriteLine($"WalkTo: {Me.Location} != {targetLocation}");

            StepToward(targetLocation);
        }
    }

    public static void WalkTo(ushort xloc, ushort yloc)
    {
        WalkTo(new Location2D(xloc, yloc));
    }

    public static void Cook(ModelId rawFoodType, string campfireTile)
    {
        var rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();

        while (rawFood != null)
        {
            Use(rawFood);
            WaitForTarget();
            TargetTile(campfireTile);

            WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
            Wait(500);

            rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();
        }
    }

    public static void Cook(ModelId rawFoodType, Item cookingPlace)
    {
        var rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();

        while (rawFood != null)
        {
            Use(rawFood);
            WaitForTarget();
            Target(cookingPlace);

            WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
            Wait(500);

            rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();
        }
    }

    public static void Cook()
    {
        var cookingPlace = Items.OfType(ItemTypes.CookingPlaces).OnGround().MaxDistance(Me.Location, 2).First();

        string cookingPlaceTile = null;
        if (cookingPlace == null)
        {
            Log("Cooking place not found, specify a cooking place tile");
            cookingPlaceTile = Info();
            if (cookingPlaceTile == null)
            {
                Log("No cooking place tile found");
                return;
            }
        }

        var rawFood = Items.OfType(ItemTypes.RawFood).InContainer(Me.BackPack).First();
        while (rawFood != null)
        {
            if (cookingPlace == null)
                Cook(rawFood.Type, cookingPlaceTile);
            else
                Cook(rawFood.Type, cookingPlace);
            rawFood = Items.OfType(ItemTypes.RawFood).InContainer(Me.BackPack).First();
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
        var items = Items.InContainer(container).ToArray();
        while (items.Any())
        {
            Log($"Looting, {items.Length} items remaining ");
            Pickup(items.First());
            items = Items.InContainer(container).ToArray();
        }

        Log("Looting finished");
    }

    public static void Pickup(Item item)
    {
        MoveItem(item, Me.BackPack);
    }

    public static void MoveItems(IEnumerable<Item> items, Item targetContainer)
    {
        foreach (var item in items)
        {
            MoveItem(item, targetContainer);
        }
    }

    public static void MoveItem(Item item, Item targetContainer)
    {
        var refreshedItem = Items.RefreshItem(item);
        if (refreshedItem == null)
        {
            Log($"Cannot move item {item.Type}, it disappeared.");
            return;
        }
        item = refreshedItem;

        DragItem(item);
        Wait(200);

        DropItem(item, targetContainer);
        Wait(200);
    }

    public static void PickupFromGround(ushort type)
    {
        PickupFromGround((ModelId) type);
    }

    public static void PickupFromGround(params ModelId[] type)
    {
        var itemsOnGround = Items.OfType(type).OnGround();
        foreach (var item in itemsOnGround)
        {
            Log($"Picking up {item.Type}");
            Pickup(item);
            Wait(250);
        }
    }

    public static void Kill(Item subject)
    {
        WarModeOn();
        Attack(subject);

        while (subject != null)
        {
            if (subject.GetDistance(Me.Location) > 1)
                StepToward(subject);
            subject = Items.RefreshItem(subject);
        }

        Wait(1000);
        var bodyOfSubject = Items.OfType(ItemTypes.RippableBodies).OrderByDistance(Me.Location).First();

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
                Items.OfType(ItemTypes.MassKillSubjects)
                    .MaxDistance(Me.Location, 20)
                    .OrderByDistance(Me.Location)
                    .First();
            if (subject == null)
            {
                Log("nothing to kill");
                break;
            }

            Kill(subject);
        } while (true);
    }

    public static void ToggleNearestDoor()
    {
        var nearestDoor = Items.OfType(ItemTypes.Doors).MaxDistance(Me.Location, 5).OrderByDistance(Me.Location).First();
        if (nearestDoor != null)
            Use(nearestDoor);
        else
            Log("Cannot find doors");
    }

    public static void OpenNearestDoor()
    {
        var nearestDoor =
            Items.OfType(ItemTypes.ClosedDoors).MaxDistance(Me.Location, 5).OrderByDistance(Me.Location).First();
        if (nearestDoor != null)
            Use(nearestDoor);
        else
            Log("Cannot find closed doors");
    }

    public static void CloseNearestDoor()
    {
        var nearestDoor =
            Items.OfType(ItemTypes.OpenDoors).MaxDistance(Me.Location, 5).OrderByDistance(Me.Location).First();
        if (nearestDoor != null)
            Use(nearestDoor);
        else
            Log("Cannot find open doors");
    }

    public static void OpenBankFromHouseMenu()
    {
        var menu = Items.OfType(ItemTypes.HouseMenu).OrderByDistance(Me.Location).First();

        if (menu != null)
        {
            Use(menu);
            WaitForGump();
            SelectGumpButton("Otevrit banku", GumpLabelPosition.Before);
        }
        else
            Log("Cannot find HouseMenu");
    }
}