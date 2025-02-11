#load "common.csx"
#load "colors.csx"
#load "Specs.csx"
#load "ignore.csx"
#load "equip.csx"
#load "warehouse.csx"

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
    private static SpeechJournal journal = UO.CreateSpeechJournal();

    public static Warehouse Warehouse { get; set; } = new Warehouse();

    public static ItemSpec UselessLoot { get; } = new[]
    {
        Specs.Club, Specs.SpruceArrow, Specs.TightBoots, Specs.HorseShoes,
        Specs.Torsos, Specs.Rocks, Specs.Corpse,
        // ignoring some "invisible" items
        Specs.Hairs
    };
    
    public static ItemSpec InterestingLoot { get; set; } = new[]
    {
        Specs.Gold, Specs.Regs, Specs.Gem, Specs.Ammunition,
        Specs.MagickyPytlik, Specs.MagickyVacek, Specs.TajemnaMapa,
        Specs.Food, Specs.VanocniBanka, Specs.KamenOhne
    };
    
    public static ItemSpec DungeonLoot { get; set; } = new[]
    {
        Specs.Gold, Specs.Regs, Specs.Gem, Specs.Ammunition,
        Specs.MagickyPytlik, Specs.MagickyVacek, Specs.TajemnaMapa, Specs.VanocniBanka, Specs.VelikonocniVejce,
        Specs.KamenOhne, Specs.ShadowOre
    };
    
    
    public static bool CollectBodiesEnabled { get; set; } = false;
    public static ObjectId? MesecNaSrnciOci { get; set; }
    
    public static MobileSpec NotRippableCorpses { get; set; } = new[] { Specs.Mounts };

    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    public static ItemSpec IgnoredLoot { get; set; } = UselessLoot;
    public static ItemSpec AllowedLoot { get; set; } = null;
    public static ItemSpec OnGroundLoot { get; set; } = InterestingLoot;
    public static ItemSpec HumanCorpseLoot { get; set; } = OnGroundLoot;

    public static ObjectId? LootContainerId { get; set; }
    public static ItemSpec LootContainerSpec { get; set; }
    public static ItemSpec KnivesSpec { get; set; } = Specs.Knives;
    
    public static event Action<ObjectId> CorpseRipped;
    public static event Action<ObjectId> CorpseLooted;
    
    public static IgnoredItems lootedCorpses = new IgnoredItems();
    public static IgnoredItems rippedCorpses = new IgnoredItems();

    public static IEnumerable<Corpse> GetLootableCorpses()
    {
        var corpses = UO.Corpses
            .MaxDistance(20)
            .Where(x => !lootedCorpses.IsIgnored(x))
            .OrderByDistance().ToArray();

        return corpses;
    }

    public static IEnumerable<Corpse> GetUnrippedCorpses()
    {
        var corpses = UO.Corpses
            .MaxDistance(20)
            .Where(x => !rippedCorpses.IsIgnored(x.Id))
            .OrderByDistance().ToArray();

        return corpses;
    }
    
    private static EquipmentSet previousEquipmentSet;
    private static EquipmentSet ripEquipmentSet;
    
    public static void RipAndLoot(Corpse corpse)
    {
        var handEquipmentSet = Equip.GetHand();
        if (handEquipmentSet.Any())
        {
            if (previousEquipmentSet != null)
                Trace.Log($"Previous equipment: {previousEquipmentSet.ToString() ?? "null"}");
            else
                Trace.Log("No previous equipment");
                
            Trace.Log($"Current equipment: {handEquipmentSet}");            
            previousEquipmentSet = handEquipmentSet;
        }
        else
            UO.Log("Cannot find equipment item.");


        bool humanCorpseLooting = Specs.Player.Matches(corpse.CorpseType);

        try
        {
            if (!humanCorpseLooting)
            {
                Rip(corpse);
                Loot(corpse);
            }
            else
            {
                Loot(corpse);
                Rip(corpse);
            }
        }
        catch (Exception ex)
        {
            UO.Log($"Cannot loot corpse: {ex.Message}");
        }

        if (previousEquipmentSet != null)
        {
            UO.Wait(50);
            Equip.Set(previousEquipmentSet);
        }

        LootGround();
    }
    
    public static void RipAndLootNearest()
    {
        var corpses = GetLootableCorpses().ToArray();
        
        if (!corpses.Any())
            corpses = GetUnrippedCorpses().ToArray();

        var corpse = corpses.MaxDistance(3).FirstOrDefault();

        if (corpse != null)
        {
            var currentEquipmentSet = Equip.GetHand();
            if (previousEquipmentSet == null || ripEquipmentSet == null || !ripEquipmentSet.Equals(currentEquipmentSet))
            {
                previousEquipmentSet = currentEquipmentSet;
            }

            try
            {
                bool humanCorpseLooting = Specs.Player.Matches(corpse.CorpseType);

                if (humanCorpseLooting)
                {
                    Loot(corpse);
                    Rip(corpse);
                }
                else
                {
                    Rip(corpse);
                    Loot(corpse);
                }

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
                if (previousEquipmentSet != null)
                {
                    ripEquipmentSet = Equip.GetHand();
                    Equip.Set(previousEquipmentSet);
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
    
    public static void CollectBody()
    {
        var corpse = UO.Corpses.OrderByDistance().FirstOrDefault();
        if (corpse != null)
            CollectBody(corpse);
        else
            UO.Console.Important("No corpse found");
    }
    
    private static void CollectBody(Corpse body)
    {
        if (!CollectBodiesEnabled)
            return;
    
        if (Specs.Srnec.Matches(body))
        {
            if (!MesecNaSrnciOci.HasValue)
            {
                UO.Use(UO.Me.BackPack);
                UO.Wait(1000);
                var backpackItems = UO.Items.InBackPack();
                foreach (var item in backpackItems)
                {
                    UO.Click(item);
                }
                UO.Wait(1000);
                var mesec = UO.Items.InBackPack().Where(x => x.Name.Equals("Mesec na srnci oci")).FirstOrDefault();
                if (mesec == null)
                {
                    UO.Console.Error("Cannot find 'Mesec na srnci oci'");
                    CollectBodiesEnabled = false;
                }
                else
                {
                    MesecNaSrnciOci = mesec.Id;
                }
            }
            
            if (MesecNaSrnciOci.HasValue)
            {
                UO.ClearTargetObject();
                UO.Use(MesecNaSrnciOci.Value);
                UO.WaitForTarget();
                UO.Target(body);
                UO.Wait(1000);
            }
        }
    }
    
    private static IEnumerable<Item> GetLootableItems(Item corpse) =>
        UO.Items.InContainer(corpse)
                .Where(i => IsLootable(i));
        
    private static bool IsLootable(Item item)
        => AllowedLoot != null ? AllowedLoot.Matches(item) : !IgnoredLoot.Matches(item);

    private static bool IsLootableFromHuman(Item item)
        => AllowedLoot != null ? AllowedLoot.Matches(item) : !IgnoredLoot.Matches(item) && HumanCorpseLoot.Matches(item);

    public static void HighlightLootableCorpses(IEnumerable<Item> lootableCorpses)
    {
        UO.ClientPrint($"{lootableCorpses.Count()} corpses to loot remaining");
        foreach (var corpse in lootableCorpses)
        {
            int lootableItemsCount = GetLootableItems(corpse).Count();
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
            if (LootContainerId.HasValue)
            {
                lootContainer = UO.Items[LootContainerId.Value];
                if (lootContainer != null 
                    && (lootContainer.Id != UO.Me.BackPack.Id
                    && lootContainer.ContainerId != UO.Me.BackPack.Id))
                {
                    UO.ClientPrint($"Selected looting container must be in backpack! Trying to select another container.",
                        UO.Me, Colors.Red);
                    lootContainer = null;                        
                }
            }
            
            if (LootContainerSpec != null)
            {
                var potentialLootContainer = UO.Items
                    .InContainer(UO.Me.BackPack)
                    .Matching(LootContainerSpec)
                    .FirstOrDefault();
                
                if (potentialLootContainer != null)
                {
                    if (lootContainer == null)
                    {
                        lootContainer = potentialLootContainer;
                        UO.Log($"{LootContainerId?.ToString() ?? "null"} {lootContainer.Id}");
                        if (LootContainerId != lootContainer.Id)
                        {
                            UO.ClientPrint("Default loot container selected from your backpack.");
                            UO.Click(lootContainer);
                        }
                    }
                    else if (lootContainer.Id == UO.Me.BackPack.Id)
                    {
                        // override backpack with a new container matching LootContainerSpec
                        lootContainer = potentialLootContainer;
                    }
                }
            }
                            
            if (lootContainer == null)
            {
                lootContainer = Common.AskForContainer("Select loot container", UO.Me, Colors.Red);
                if (lootContainer == null)
                {
                    UO.ClientPrint("Loot container selection canceled, looting to backpack.", UO.Me, Colors.Red);
                    UO.ClientPrint("Type `,reload` To select other other looting container.");
                    lootContainer = UO.Me.BackPack;
                }
            }
            
            LootContainerId = lootContainer.Id;
            
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
            UO.DragItem(item);
            UO.Wait(10);
            UO.DropItem(item, LootContainer);
            UO.Wait(10);
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

        UO.Use(container);
        if (!Common.WaitForContainer())
        {
            UO.Log("Cannot open body, maybe it is not possible to loot the body");
            return;
        }

        UO.ClientPrint($"Number of items in container: {UO.Items.InContainer(container).Count()}");

        Func<Item, bool> isLootableFunc = IsLootable;
        if (container is Corpse containerCorpse)
        {
            UO.Log(containerCorpse.CorpseType.ToString());
            if (Specs.Player.Matches(containerCorpse.CorpseType))
            {
                isLootableFunc = IsLootableFromHuman;
            }
        }

        foreach (var itemToPickup in UO.Items.InContainer(container).ToArray())
        {
            if (container.GetDistance(UO.Me.Location) > 4)
            {
                UO.ClientPrint("corpse too far away, cancelling loot", "looting", UO.Me);
                return;
            }
        
            if (isLootableFunc(itemToPickup))
            {
                Trace.Log($"Looting {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
                UO.DragItem(itemToPickup);
                UO.Wait(10);
                UO.DropItem(itemToPickup, LootContainer);
                UO.Wait(10);

                if (journal.Contains("Ne tak rychle!"))
                {
                    journal.Delete();
                    UO.Wait(4000);
                }
            }
            else
            {
                Trace.Log($"Ignoring {Specs.TranslateToName(itemToPickup)} ({itemToPickup.Amount})");
            }
        }

        UO.ClientPrint("Looting finished", UO.Me, Colors.Green);
        lootedCorpses.Ignore(container);
        CorpseLooted?.Invoke(container.Id);
    }

    public static bool Rip(Corpse corpse)
    {
        Trace.Log($"Start Rip.");
        if (rippedCorpses.IsIgnored(corpse.Id))
        {
            UO.ClientPrint($"{Specs.TranslateToName(corpse)} already ripped.");
            return true;
        }
    
        if (NotRippableCorpses.Matches(corpse))
        {
            UO.ClientPrint($"Not ripping {Specs.TranslateToName(corpse)}, it is ignored");
            return true;
        }
    
        try
        {
            UO.WaitTargetObject(corpse);
            Trace.Log($"Use knife.");
            if (!UO.TryUse(KnivesSpec))
            {
                UO.ClientPrint("Cannot find any knife", UO.Me);
                return false;
            }
          
            bool result = false;
            
            Trace.Log($"Wait for journal.");
            journal
                .When("Rozrezal jsi mrtvolu.", () => result = true)
                .When("Rozrezala jsi mrtvolu.", () => result = true)
                .When("You carve away some meat.", " You carve the corpse but find nothing usefull.", () => result = true)
                .When("Jsi paralyzovan", "Jsi paralyzovana", "You are frozen and can not move.", "you can't reach anything in your state.", () => 
                {
                    result = false;
                    throw new InvalidOperationException("I'm paralyzed, cannot loot.");
                })
                .When("Unexpected target info", () => 
                {
                    UO.Log("Warning: unexpected target info when targeting a body");
                    UO.Log(corpse.ToString());
                    result = false;
                })
                .WaitAny();
            
            Trace.Log($"Wait for journal finished.");
            if (result)
            {
                Trace.Log($"Result true.");
                rippedCorpses.Ignore(corpse.Id);
                CorpseRipped?.Invoke(corpse.Id);
            }

            return result;
        }
        finally
        {        
            UO.ClearTargetObject();
        }
    }
    
    public static void UnloadLoot()
    {
        Common.OpenContainer(LootContainer.Id);
        Warehouse.Sort(LootContainer.Id);
        UO.ClientPrint("Loot unloading finished.");
    }
    
    public static void SetDungeonLoot()
    {
        UO.ClientPrint("Looting only items specified by Looting.DungeonLoot.");
        Looting.AllowedLoot = Looting.DungeonLoot;
        currentLootMode = LootMode.Dungeon;
    }
    
    public static void SetInterestingLoot()
    {
        UO.ClientPrint("Looting only items specified by Looting.InterestingLoot.");
        Looting.AllowedLoot = Looting.InterestingLoot;
        currentLootMode = LootMode.Interesting;
    }
    
    public static void SetAllLoot()
    {
        UO.ClientPrint("Looting all except items specified by Looting.IgnoredLoot.");
        Looting.AllowedLoot = null;
        currentLootMode = LootMode.All;
    }
    
    public static void ToggleLoot()
    {
        switch (currentLootMode)
        {
            case LootMode.Dungeon:
                SetAllLoot();
                break;
            case LootMode.All:
                SetInterestingLoot();
                break;
            case LootMode.Interesting:
                SetDungeonLoot();
                break;
            default:
                throw new NotImplementedException($"Unknown loot mode {currentLootMode}.");
        }
    }
    
    public static void IgnoreLootingCorpseCommand(string parameter)
    {
        if (parameter.StartsWith("0x"))
            parameter = parameter.Substring(2);
        int id = int.Parse(parameter, System.Globalization.NumberStyles.HexNumber);
        
        lootedCorpses.Ignore((ObjectId)id);
    }
    
    public static void IgnoreRippedCorpseCommand(string parameter)
    {
        if (parameter.StartsWith("0x"))
            parameter = parameter.Substring(2);
        int id = int.Parse(parameter, System.Globalization.NumberStyles.HexNumber);
        
        rippedCorpses.Ignore((ObjectId)id);
    }

    
    private static LootMode currentLootMode;
    
    private enum LootMode
    {
        Dungeon,
        All,
        Interesting
    }
}

UO.RegisterCommand("ripandloot", Looting.RipAndLootNearest);
UO.RegisterCommand("loot", Looting.LootNearest);
UO.RegisterCommand("loot-unload", Looting.UnloadLoot);
UO.RegisterCommand("loot-dungeon", Looting.SetDungeonLoot);
UO.RegisterCommand("loot-all", Looting.SetAllLoot);
UO.RegisterCommand("loot-interesting", Looting.SetInterestingLoot);
UO.RegisterCommand("loot-toggle", Looting.ToggleLoot);
UO.RegisterCommand("loot-ignore-ripped", Looting.IgnoreRippedCorpseCommand);
UO.RegisterCommand("loot-ignore-looted", Looting.IgnoreLootingCorpseCommand);
UO.RegisterCommand("loot-collect-body", Looting.CollectBody);