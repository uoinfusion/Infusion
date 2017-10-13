#load "Specs.csx"
#load "ignore.csx"
#load "equip.csx"

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Infusion.Commands;

public static class Looting
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of SpeechJournal so
    // you can try looting and healing in parallel
    // and you can still use journal.Delete method in both scripts at the same time.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static SpeechJournal journal = UO.CreateJournal();

    public static ItemSpec UselessLoot { get; } = new[]
    {
        Specs.Torsos, Specs.Rocks, Specs.Corpse,
        // ignoring some "invisible" items
        Specs.Hairs
    };

    public static ItemSpec IgnoredLoot { get; set; } = UselessLoot;
    public static ItemSpec OnGroundLoot { get; set; } = new[]
    {
        Specs.Gold, Specs.Regs, Specs.Gem, Specs.Bolt
    };

    public static ObjectId? LootContainerId { get; set; }
    public static ItemSpec LootContainerSpec { get; set; }
    public static ItemSpec KnivesSpec { get; set; } = Specs.Knives;
    
    public static IgnoredItems ignoredItems = new IgnoredItems();

    public static IEnumerable<Item> GetLootableCorpses()
    {
        var corpses = UO.Items
            .Matching(Specs.Corpse)
            .MaxDistance(20)
            .Where(x => !ignoredItems.IsIgnored(x))
            .OrderByDistance().ToArray();

        return corpses;
    }
    
    private static Equipment? previousEquipment;
    
    public static void RipAndLoot(Item corpse)
    {
        var handEquipment = Equip.GetHand();
        var handEquipmentItem = UO.Items[handEquipment.Id];
        if (handEquipmentItem != null)
        {
            if (!KnivesSpec.Matches(handEquipmentItem))
            {
                if (!previousEquipment.HasValue || handEquipment.Id != previousEquipment.Value.Id)
                {
                    UO.Log($"Previous equipment: {previousEquipment?.ToString() ?? "null"}");
                    UO.Log($"Current equipment: {handEquipment}");            
                    previousEquipment = handEquipment;
                }
            }
            else
                UO.Log("Holding knife, will not requip");
        }
        else
            UO.Log("Cannot find equipment item.");

        try
        {
            Rip(corpse);
            Loot(corpse);
        }
        catch (Exception ex)
        {
            UO.Log($"Cannot loot corpse: {ex.Message}");
        }

        if (previousEquipment.HasValue)
        {
            UO.Wait(100);
            Equip.Set(previousEquipment.Value);
            UO.Wait(100);
        }

        LootGround();
    }
    
    public static void RipAndLootNearest()
    {
        var corpses = GetLootableCorpses().ToArray();
        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            var handEquipment = Equip.GetHand();
            var handEquipmentItem = UO.Items[handEquipment.Id];
            if (handEquipmentItem != null)
            {
                if (!KnivesSpec.Matches(handEquipmentItem))
                {
                    if (!previousEquipment.HasValue || handEquipment.Id != previousEquipment.Value.Id)
                    {
                        UO.Log($"Previous equipment: {previousEquipment?.ToString() ?? "null"}");
                        UO.Log($"Current equipment: {handEquipment}");            
                        previousEquipment = handEquipment;
                    }
                }
                else
                    UO.Log("Holding knife, will not requip");
            }
            else
                UO.Log("Cannot find equipment item.");

            try
            {
                Rip(corpse);
                Loot(corpse);
                if (corpses.Length - 1 > 0)
                    HighlightLootableCorpses(corpses.Except(new[] { corpse }));
                else
                    UO.ClientPrint($"No more corpses to loot remaining");
            }
            catch (Exception ex)
            {
                UO.Log($"Cannot loot corpse: {ex.Message}");
            }
            finally
            {
                // It seems that re-wearing an item directly
                // after ripping a body and right before
                // looting may crash the game client.
                if (previousEquipment.HasValue)
                {
                    UO.Wait(100);
                    Equip.Set(previousEquipment.Value);
                    UO.Wait(100);
                }
            }
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

        LootGround();
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

    private static Item lootContainer;
    
    public static Item LootContainer
    {
        get
        {
            if (lootContainer == null || UO.Items.Refresh(lootContainer) == null)
            {
                if (LootContainerId.HasValue)
                {
                    lootContainer = UO.Items[LootContainerId.Value];
                }
                
                if (lootContainer == null && LootContainerSpec != null)
                {
                    lootContainer = UO.Items
                        .InContainer(UO.Me.BackPack)
                        .Matching(LootContainerSpec)
                        .FirstOrDefault();
                }
                
                if (lootContainer == null)
                {
                    lootContainer = UO.Me.BackPack;
                }
            }
            
            return lootContainer;
        }
    }

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

    public static void Loot(ObjectId containerId)
    {
        var container = UO.Items[containerId];
        if (container == null)
            UO.ClientPrint($"Cannot find {containerId} container");

        Loot(container);
    }

    public static void Loot(Item container)
    {
        var originalLocation = UO.Me.Location;

        UO.OpenContainer(container);

        UO.ClientPrint($"Number of items in container: {UO.Items.InContainer(container).Count()}");

        foreach (var itemToPickup in UO.Items.InContainer(container).ToArray())
        {
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
        ignoredItems.Ignore(container);
    }

    public static bool Rip(Item container)
    {
        UO.ClientPrint("Ripping");
        if (!UO.TryUse(KnivesSpec))
        {
            UO.Alert("Cannot find any knife");
            return false;
        }

        UO.WaitForTarget();
        UO.Target(container);
        
        bool result = false;
        
        journal
            .When("Rozrezal jsi mrtvolu.", () => result = true)
            .When("Unexpected target info", () => 
            {
                UO.Log("Warning: unexpected target info when targeting a body");
                UO.Log(container.ToString());
                result = false;
            })
            .WaitAny();
            
        return true;
    }

}

UO.RegisterCommand("ripandloot", Looting.RipAndLootNearest);
UO.RegisterCommand("loot", Looting.LootNearest);
