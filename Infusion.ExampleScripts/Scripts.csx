#load "ItemTypes.csx"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using static Infusion.Proxy.LegacyApi.Legacy;

public static class Scripts
{
    public static string[] skipOre = {"Copper Ore"};

    public static DateTime lastFailedLumberjackingAttempt;

    public static DateTime LastCheckTime;

    public static readonly TimeSpan FailedLumberjackingWait = TimeSpan.FromSeconds(2);

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

    public static ushort MaxWeight => 480;

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

    public static void Fish()
    {
        var tileInfo = Info();

        Fish(tileInfo);
    }

    public static void HarvestTree(string tileInfo)
    {
        Program.Diagnostic.Debug($"Lumberjacking {tileInfo}");

        var treeHarvestable = true;

        var sinceLastFail = DateTime.UtcNow - lastFailedLumberjackingAttempt;
        Program.Diagnostic.Debug($"Since last lumberjacking fail: {sinceLastFail}");
        if (sinceLastFail < FailedLumberjackingWait)
        {
            var waitSpan = FailedLumberjackingWait - sinceLastFail;
            Program.Diagnostic.Debug(
                $"{DateTime.UtcNow:T} Waiting due to lumberjacking fail: {waitSpan}, {(int) waitSpan.TotalMilliseconds} ms");
            Wait(waitSpan);
            Program.Diagnostic.Debug($"{DateTime.UtcNow:T} waiting finished");
        }

        while (treeHarvestable)
        {
            Wait(1000);
            Checks();
            DeleteJournal();
            Program.Diagnostic.Debug("Using any hatchet");
            UseType(ItemTypes.Hatchets);
            WaitForTarget();
            TargetTile(tileInfo);

            WaitForJournal("You put", "Drevo se nepodarilo", "of a way to use", "immune", "There are no logs",
                "You cannot reach that", "is attacking you");

            treeHarvestable =
                !InJournal("of a way to use", "immune", "There are no logs here to chop.", "You cannot reach that");
            Checks();

            if (!treeHarvestable)
            {
                lastFailedLumberjackingAttempt = DateTime.UtcNow;
                Program.Diagnostic.Debug($"Last lumberjacking fail: {lastFailedLumberjackingAttempt:T}");
            }
            else if (InJournal("Jeste nemuzes pouzit skill."))
            {
                Program.Diagnostic.Debug("waiting for skill");
                Wait(5000);
                Checks();
            }
        }
    }

    public static void Checks()
    {
        AttackCheck();
        HealthCheck();
        LightCheck();
        PauseCheck();

        LastCheckTime = DateTime.UtcNow;
    }

    private static DateTime lastHealthWarning;

    private static void HealthCheck()
    {
        if (Me.CurrentHealth < Me.MaxHealth / 10)
        {
            var now = DateTime.UtcNow;
            if (lastHealthWarning.AddSeconds(10) < now)
            {
                Program.Console.Critical($"Low health: {Me.CurrentHealth}/{Me.MaxHealth}");
                lastHealthWarning = now;
            }
        }
    }

    private static bool harvestingPaused;

    private static void PauseCheck()
    {
        if (harvestingPaused)
        {
            Program.Console.Critical("Harvesting paused");

            do
            {
                Wait(1000);
            } while (harvestingPaused);

            Program.Console.Critical("Harvesting resumed");
        }
    }

    private static void LightCheck()
    {
        if (InJournal(LastCheckTime, "Je spatne videt"))
        {
            var item =
                Items.InContainer(Me.BackPack).OfType(ItemTypes.Torch).First();
            //var item =
            //    Items.InContainer(Me.BackPack).OfType(ItemTypes.Bottle).OfColor(ItemTypes.NightSightKegColor).First();
            if (item != null)
            {
                Use(item);
                Wait(1000);
            }
        }
    }

    public static IEnumerable<uint> FindAttackers()
    {
        // for tests:
        //return Items.OfType(SafeAttackers)
        //    .MaxDistance(Me.Location, 20)
        //    .Select(i => i.Id);

        return Journal
            .After(LastCheckTime)
            .ContainsAnyWord("is attacking you")
            .Where(e => !SafeAttackers.Contains(e.Type))
            .Select(e => e.SpeakerId);
    }

    public static IEnumerable<uint> UpdateAttackers(IEnumerable<uint> currentAttackers)
    {
        var newAttackers = FindAttackers();
        var myLocation = (Location2D) Me.Location;

        var relevantAttackers = currentAttackers.Select(id => Items.Get(id))
            .Where(i => i != null)
            .MaxDistance(myLocation, 30)
            .Select(i => i.Id);

        return newAttackers.Concat(relevantAttackers);
    }

    public static void AttackCheck()
    {
        var updatedAttackers = UpdateAttackers(Attackers);

        Attackers = updatedAttackers.ToArray();

        var escapeMode = Attackers.Any();
        if (escapeMode != EscapeMode)
        {
            EscapeMode = escapeMode;
            if (EscapeMode)
            {
                Log($"{Attackers.Length} attackers detected -> escape mode");
                throw new AttackException();
            }
            Log("No attackers -> leaving escape mode.");
        }
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

    public static void MoveItems(IEnumerable<Item> items, ushort amount, Item targetContainer)
    {
        foreach (var item in items)
        {
            if (amount == 0)
                break;

            if (item.Amount <= amount)
            {
                MoveItem(item, targetContainer);
                amount -= item.Amount;
            }
            else
            {
                MoveItem(item, amount, targetContainer);
                amount = 0;
            }
        }
    }

    public static void Eat()
    {
        var itemsToEat = Items.OfType(ItemTypes.Food);
        var journalCheck = DateTime.UtcNow;

        foreach (var foodItem in itemsToEat)
        {
            var food = foodItem;
            while (food != null && food.Amount > 0)
            {
                Use(food);
                WaitForJournal("You eat some", "You are simply too full", "You can't think of a way to use that item");
                Wait(200);

                if (InJournal(journalCheck, "You are simply too full", "You can't think of a way to use that item"))
                    return;

                journalCheck = DateTime.UtcNow;
                food = Items.RefreshItem(food);
            }
        }
    }

    public static void MoveItem(Item item, Item targetContainer)
    {
        MoveItem(item, item.Amount, targetContainer);
    }

    public static void MoveItem(Item item, ushort amount, Item targetContainer)
    {
        var refreshedItem = Items.RefreshItem(item);
        if (refreshedItem == null)
        {
            Log($"Cannot move item {item.Type}, it disappeared.");
            return;
        }
        item = refreshedItem;

        Program.Diagnostic.Debug($"Dragging {amount} item {item}");
        DragItem(item, amount);
        Wait(200);

        DropItem(item, targetContainer);
        Program.Diagnostic.Debug($"Dropping {amount} item {item}");
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
            Items.OfType(ItemTypes.ClosedDoors).MaxDistance(Me.Location, 2).OrderByDistance(Me.Location).First();
        if (nearestDoor != null)
            Use(nearestDoor);
        else
            Log("Cannot find closed doors");
    }

    public static void CloseNearestDoor()
    {
        var nearestDoor =
            Items.OfType(ItemTypes.OpenDoors).MaxDistance(Me.Location, 2).OrderByDistance(Me.Location).First();
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