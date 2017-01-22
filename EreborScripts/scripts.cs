using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

    private static readonly string[] AfkNames =
    {
        "desttro", "elbereth", "finn", "gothmog", "houba", "iustus", "myke", "yavanna",
        "nightmare"
    };

    private static readonly string[] AfkMessages = {"afk", "kontrola"};

    private static DateTime lastCheckTime;

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
        if (mapLine.StartsWith("walk: "))
        {
            var parameters = mapLine.Substring("walk: ".Length).Split(',').Select(x => ushort.Parse(x.Trim())).ToArray();
            Log($"Walking to: {parameters[0]}, {parameters[1]}");
            WalkTo(parameters[0], parameters[1]);
        }

        try
        {
            if (!EscapeMode)
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
                else if (mapLine.StartsWith("masskill"))
                {
                    MassKill();
                }
            }
            else
            {
                Log($"In escape mode, just check attackers, and do no other action.");
                AttackCheck();
            }
        }
        catch (AttackException e)
        {
            // just continue
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

    public static readonly TimeSpan FailedLumberjackingWait = TimeSpan.FromSeconds(5);

    public static void HarvestTree(string tileInfo)
    {
        Log($"Lumberjacking {tileInfo}");

        var treeHarvestable = true;

        var sinceLastFail = DateTime.UtcNow - lastFailedLumberjackingAttempt;
        Log($"Since last lumberjacking fail: {sinceLastFail}");
        if (sinceLastFail < FailedLumberjackingWait)
        {
            var waitSpan = FailedLumberjackingWait - sinceLastFail;
            Log($"{DateTime.UtcNow:T} Waiting due to lumberjacking fail: {waitSpan}, {(int)waitSpan.TotalMilliseconds} ms");
            Wait(waitSpan);
            Log($"{DateTime.UtcNow:T} waiting finished");
        }

        while (treeHarvestable)
        {
            Wait(1000);
            DeleteJournal();
            Log("Using any hatchet");
            UseType(ItemTypes.Hatchets);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs",
                "You cannot reach that");

            Checks();

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

    private static void Checks()
    {
        AfkCheck();
        AttackCheck();

        lastCheckTime = DateTime.UtcNow;
    }

    public static readonly ModelId[] SafeAttackers =
    {
        ItemTypes.Cow,
        ItemTypes.Cow2,
        ItemTypes.Bird,
        ItemTypes.Dog,
        ItemTypes.Rabbit,
        ItemTypes.Rat
    };

    public static uint[] Attackers = {};

    public static bool EscapeMode;

    public static IEnumerable<uint> FindAttackers()
    {
        // for tests:
        //return Items.OfType(SafeAttackers)
        //    .MaxDistance(Me.Location, 20)
        //    .Select(i => i.Id);

        return Journal
            .After(lastCheckTime)
            .ContainsAnyWord("is attacking you")
            .Where(e => !SafeAttackers.Contains(e.Type))
            .Select(e => e.SpeakerId);
    }

    public static IEnumerable<uint> UpdateAttackers(IEnumerable<uint> currentAttackers)
    {
        var newAttackers = FindAttackers();
        var myLocation = (Location2D)Me.Location;

        var relevantAttackers = currentAttackers.Select(id => Items[id])
            .MaxDistance(myLocation, 30)
            .Select(i => i.Id);

        return newAttackers.Concat(relevantAttackers);
    }

    public static void AttackCheck()
    {
        var updatedAttackers = UpdateAttackers(Attackers);

        Attackers = updatedAttackers.ToArray();

        bool escapeMode = Attackers.Any();
        if (escapeMode != EscapeMode)
        {
            EscapeMode = escapeMode;
            if (EscapeMode)
            {
                Log($"{Attackers.Length} attackers detected -> escape mode");
                throw new AttackException();
            }
            else
                Log("No attackers -> leaving escape mode.");
        }
    }

    private static void AfkCheck()
    {
        var afkAlertRequired = Journal
            .After(lastCheckTime)
            .ByAnyName(AfkNames)
            .ContainsAnyWord(AfkMessages).Any();

        if (afkAlertRequired)
        {
            AfkAlert();
        }
    }

    public static void AfkAlert()
    {
        while (true)
        {
            if (InJournal("tak zpet do prace"))
                break;
            DeleteJournal();

            System.Media.SystemSounds.Asterisk.Play();

            Wait(1000);
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

public class AttackException : Exception
{
}
