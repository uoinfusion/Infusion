#load "Specs.csx"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Infusion.Commands;

public static class Looting
{
    public static ItemSpec UselessLoot { get; } = new[] { Specs.Torsos, Specs.Rocks, Specs.Corpse };
    public static ItemSpec IgnoredLoot { get; set; } = UselessLoot;
    public static ItemSpec OnGroundLoot { get; set; } = new[] { Specs.Gold, Specs.Regs, Specs.Gem, Specs.Bolt };
    public static ObjectId? LootContainerId { get; set; }

    private static object alreadyLootedItemsLock = new object();
    private static Dictionary<ObjectId, Item> alreadyLootedItems = new Dictionary<ObjectId, Item>();

    static Looting()
    {
        UO.Events.ItemEnteredView += OnItemEnteredView;
    }

    private static void OnItemEnteredView(object sender, ItemEnteredViewArgs args)
    {
        lock (alreadyLootedItemsLock)
        {
            if (alreadyLootedItems.TryGetValue(args.NewItem.Id, out Item item) &&
                (item.Location != args.NewItem.Location || item.Type != args.NewItem.Type))
            {
                UO.ClientPrint($"Warning: removing ignored item, current item: {item}, new item: {args.NewItem}");
                alreadyLootedItems.Remove(args.NewItem.Id);
            }
        }
    }

    private static void Ignore(Item item)
    {
        lock (alreadyLootedItemsLock)
        {
            alreadyLootedItems[item.Id] = item;
        }
    }

    private static bool IsIgnored(Item item)
    {
        lock (alreadyLootedItemsLock)
        {
            if (alreadyLootedItems.TryGetValue(item.Id, out Item ignoredContainer))
            {
                if (ignoredContainer.Location != item.Location)
                {
                    UO.ClientPrint($"Warning: Ignored: {ignoredContainer}");
                    UO.ClientPrint($"Found with same id: {item}");
                    return false;
                }

                return true;
            }

            return false;
        }
    }


    public static IEnumerable<Item> GetLootableCorpses()
    {
        var corpses = UO.Items
            .Matching(Specs.Corpse)
            .MaxDistance(20)
            .Where(x => !IsIgnored(x))
            .OrderByDistance().ToArray();

        return corpses;
    }

    public static void RipAndLootNearest()
    {
        LootGround();

        var corpses = GetLootableCorpses().ToArray();
        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            Rip(corpse);
            Loot(corpse);
            if (corpses.Length - 1 > 0)
                HighlightLootableCorpses(corpses.Except(new[] { corpse }));
            else
                UO.ClientPrint($"No more corpses to loot remaining");
        }
        else
        {
            if (corpses.Length > 0)
            {
                HighlightLootableCorpses(corpses);
                UO.ClientPrint($"No corpse reacheable but {corpses.Length} corpses to loot remaining");
            }
            else
                UO.ClientPrint("no corpse found");
        }
    }

    public static void HighlightLootableCorpses(IEnumerable<Item> lootableCorpses)
    {
        UO.ClientPrint($"{lootableCorpses.Count()} corpses to loot remaining");
        foreach (var corpse in lootableCorpses)
        {
            int lootableItemsCount = UO.Items.InContainer(corpse)
                .NotMatching(IgnoredLoot).Count();
            UO.ClientPrint($"--{lootableItemsCount}--", "System", corpse.Id, corpse.Type,
                SpeechType.Speech, Colors.Green, log: false);
        }
    }

    public static void LootNearest()
    {
        LootGround();

        var corpses = GetLootableCorpses().ToArray();
        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            Loot(corpse);
            if (corpses.Length - 1 > 0)
                HighlightLootableCorpses(corpses.Except(new[] { corpse }));
            else
                UO.ClientPrint($"No more corpses to loot remaining");
        }
        else
        {
            if (corpses.Length > 0)
            {
                UO.ClientPrint($"No corpse reacheable but {corpses.Length} corpses to loot remaining");
                HighlightLootableCorpses(corpses);
            }
            else
                UO.ClientPrint("no corpse found");
        }
    }

    public static Item LootContainer => LootContainerId.HasValue ? UO.Items[LootContainerId.Value] ?? UO.Me.BackPack : UO.Me.BackPack;

    public static void LootGround()
    {
        var itemsOnGround = UO.Items.Matching(OnGroundLoot).MaxDistance(3).OrderByDistance().ToArray();
        if (itemsOnGround.Length > 0)
            UO.ClientPrint("Looting ground");

        foreach (var item in itemsOnGround)
        {
            if (!UO.TryMoveItem(item, LootContainer))
            {
                UO.ClientPrint("Cannot pickup item, cancelling ground loot");
                break;
            }
        }
    }

    public static void Loot()
    {
        var container = UO.AskForItem();
        if (container != null)
        {
            Loot(container);
        }
        else
            UO.ClientPrint("no container for loot");
    }

    private static GameJournal journal = UO.CreateJournal();

    public static void Loot(Item container)
    {
        var originalLocation = UO.Me.Location;

        UO.OpenContainer(container);

        UO.ClientPrint($"Number of items in container: {UO.Items.InContainer(container).Count()}");

        foreach (var itemToPickup in UO.Items.InContainer(container).ToArray())
        {
            if (originalLocation != UO.Me.Location)
                throw new CommandInvocationException("Cancel looting, player has moved.");

            if (!IgnoredLoot.Matches(itemToPickup))
            {
                UO.ClientPrint($"Looting {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
                if (!UO.TryMoveItem(itemToPickup, LootContainer))
                {
                    UO.ClientPrint("Cannot pickup an item, cancelling loot");
                    return;
                }

                UO.Wait(25);
                if (journal.Contains("Ne tak rychle!"))
                {
                    journal.Delete();
                    UO.Wait(4000);
                }
            }
            else
            {
                UO.ClientPrint($"Ignoring {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
            }
        }

        UO.ClientPrint($"Looting finished, ignoring corpse {container.Id:X8}");
        Looting.Ignore(container);
    }

    public static void Rip(Item container)
    {
        UO.ClientPrint("Ripping");
        var itemInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault() ?? UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
        UO.Use(Specs.Knives);

        UO.WaitForTarget();
        UO.Target(container);
        journal.WaitAny("Rozrezal jsi mrtvolu.");

        if (itemInHand != null)
        {
            UO.TryWear(itemInHand, Layer.OneHandedWeapon);
            UO.Wait(100);
        }
    }

}

UO.RegisterCommand("ripandloot", Looting.RipAndLootNearest);
UO.RegisterCommand("loot", Looting.LootNearest);
