#load "Specs.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.Commands;

public static class Items
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();
    private static EventJournal journal = UO.CreateEventJournal();

    public static string GetName(Item item)
        => string.IsNullOrEmpty(item.Name) ? Specs.TranslateToName(item) : item.Name;
    

    public static bool Drag(Item item)
        => Drag(item, 1);
    
    public static bool Drag(Item item, int amount)
    {
        UO.DragItem(item, amount);
        var dragResult = UO.WaitForItemDragged(item.Id);
        switch (dragResult)
        {
            case DragResult.Success:
                return true;
            case DragResult.Timeout:
                UO.ClientPrint($"Timeout when dragging {GetName(item)}");
                return false;
            default:
                UO.ClientPrint($"Cannot drag {GetName(item)}, reason: {dragResult}"); 
                return false;
        }
    }

    public static bool Wear(Item item, Layer layer, TimeSpan? timeout = null)
    {
        bool result = false;

        if (!Drag(item))
            return false;

        UO.Wear(item, layer);
        
        journal
            .When<ItemWornEvent>(
                x => x.ItemId == item.Id && x.MobileId == UO.Me.PlayerId,
                x =>
                {
                    Trace.Log($"Item {item} worn.");
                    result = true;
                })
            .When<MoveItemRequestRejectedEvent>(x =>
            {
                UO.ClientPrint($"Wear request rejected: {GetName(item)}, reason: {x.Reason}");
                result = false;
            })
            .WhenTimeout(() =>
            {
                UO.ClientPrint($"Timeout when wearing {GetName(item)}");
                result = false;
            })
            .WaitAny(timeout);
            
        return result;
    }

    public static bool TryMoveItem(Item item, GameObject targetContainer)
        => TryMoveItem(item, item.Amount, targetContainer.Id);

    public static bool TryMoveItem(Item item, GameObject targetContainer, Location2D? location)
        => TryMoveItem(item, item.Amount, targetContainer.Id, location);

    public static bool TryMoveItem(Item item, ObjectId targetContainerId)
        => TryMoveItem(item, item.Amount, targetContainerId);

    public static bool TryMoveItem(Item item, ObjectId targetContainerId, Location2D? location)
        => TryMoveItem(item, item.Amount, targetContainerId, location);

    public static bool TryMoveItem(ObjectId itemId, ObjectId targetContainerId, Location2D? location)
        => TryMoveItem(itemId, targetContainerId, location);

    public static bool TryMoveItem(Item item, ushort amount, GameObject targetContainer)
        => TryMoveItem(item, amount, targetContainer.Id);
    
    public static bool TryMoveItem(Item item, ushort amount, ObjectId targetContainerId)
        => TryMoveItem(item, amount, targetContainerId, null);

    public static bool TryMoveItem(Item item, ushort amount, ObjectId targetContainerId, Location2D? location)
    {
        bool result = false;
    
        if (!Drag(item, amount))
            return false;
        
        if (location.HasValue)
            UO.DropItem(item.Id, targetContainerId, location.Value);
        else
            UO.DropItem(item, targetContainerId);
        
        journal
            .When<Infusion.LegacyApi.Events.ItemEnteredViewEvent>(
                e => e.NewItem.Id == item.Id,
                e =>
                {
                    result = true;
                    Trace.Log($"Item drop confirmed: {item}");
                })
            .When<MoveItemRequestRejectedEvent>(x =>
            {
                Trace.Log($"Item drop rejected: {item}");
                result = false;
            })
            .WhenTimeout(() =>
            {
                result = false;
                Trace.Log("Waiting for drop item timeout");
            })
            .WaitAny();
        
        return result;
    }
    
    public static bool Pickup(Item item)
        => TryMoveItem(item, UO.Me.BackPack);

    public static bool DropFromBackPack(ItemSpec itemSpec, bool all = false)
        => DropFromBackPack(itemSpec, UO.Me.Location, all);    

    public static bool DropFromBackPack(ItemSpec itemSpec, int amount)
        => DropFromBackPack(itemSpec, UO.Me.Location, amount);

    public static bool DropFromBackPack(ItemSpec itemSpec, Location3D location, int amount)
    {
        var potentialItems = UO.Items.Matching(itemSpec).InBackPack().ToArray();
        
        int i = 0;
        int remainingAmount = amount;
        
        while (i < potentialItems.Length && remainingAmount > 0)
        {
            var item = potentialItems[i];
            if (item.Amount > remainingAmount)
            {
                if (Drop(item, location, remainingAmount))
                    remainingAmount = 0;
            }
            else
            {
                if (Drop(item, location, item.Amount))
                    remainingAmount -= item.Amount;
            }
            
            i++;
        }

        if (remainingAmount > 0)
        {
            UO.ClientPrint($"Cannot find enough {Specs.TranslateToName(itemSpec)} in your backpack (remaining amount {remainingAmount}).");
            return false;
        }
        
        return true;
    }

    public static bool DropFromBackPack(ItemSpec itemSpec, Location3D location, bool all = false)
    {
        var potentialItems = UO.Items.Matching(itemSpec).InBackPack();
        
        if (!potentialItems.Any())
        {
            UO.ClientPrint($"Cannot find any {Specs.TranslateToName(itemSpec)} in your backpack");
            return false;
        }
        
        if (all)
        {
            foreach (var item in potentialItems)
            {
                UO.Trace.Log($"Dropping {Specs.TranslateToName(item)} ({item.Id})");
                if (!Drop(item, location, item.Amount))
                    return false;
            }
            
            return true;
        }
        else
        {
            var first = potentialItems.First();
            return Drop(first, location, first.Amount);
        }
    }

    public static bool Drop(Item item)
        => Drop(item, UO.Me.Location, item.Amount);
    
    public static bool Drop(Item item, Location3D location, int amount)
    {
        if (item != null)
        {
            UO.DragItem(item, amount);
            var result = UO.WaitForItemDragged(item.Id);
            if (result != DragResult.Success)
            {
                UO.ClientPrint($"Cannot drop item (reason: {result})");
                return false;
            }
            UO.DropItem(item, location);
            
            return true;
        }
        
        return false;
    }
    
    public static void BatchMoveAllCommand(string parameters)
    {
        int batchSize = new CommandParameterParser(parameters).ParseInt();
        
        BatchedMove(-1, batchSize);
    }
    
    public static void BatchedMove(int totalAmount, int batchSize)
    {
        UO.ClientPrint("Select item to move");
        var itemTemplate = UO.AskForItem();
        if (itemTemplate == null)
        {
            UO.ClientPrint("Moving cancelled");
            return;
        }
        
        UO.ClientPrint("Select target container");
        var targetContainer = UO.AskForItem();
        if (targetContainer == null)
        {
            UO.ClientPrint("Moving cancelled");
            return;
        }
        
        totalAmount = totalAmount > 0 ? totalAmount : itemTemplate.Amount; 
        
        while (totalAmount > 0)
        {
            var itemsToMove = UO.Items.OfColor(itemTemplate.Color).OfType(itemTemplate.Type);
            if (itemTemplate.ContainerId.HasValue)
            {
                itemsToMove = itemsToMove.InContainer(itemTemplate.ContainerId.Value);
            }
            else
            {
                itemsToMove = itemsToMove.OnGround();
            }
            
            var itemsToMoveOnSamePosition = itemsToMove.Where(x => x.Location == itemTemplate.Location).ToArray();
            if (itemsToMoveOnSamePosition.Any())
                itemsToMove = itemsToMoveOnSamePosition;
            
            var itemToMove = itemsToMove.FirstOrDefault();
            if (itemToMove == null)
            {
                UO.ClientPrint("No item to move found");
                break;
            }
            var amount = batchSize > totalAmount ? totalAmount : batchSize;
            amount = amount > itemToMove.Amount ? itemToMove.Amount : amount;
            
            UO.ClientPrint($"Moving {amount}, {totalAmount} to move");
            Items.MoveItems(itemsToMove, (ushort)amount, targetContainer);
            totalAmount -= amount;   
        }
    }
    
    public static void MoveItems(IEnumerable<Item> items, Item targetContainer)
    {
        foreach (var item in items)
        {
            TryMoveItem(item, targetContainer);
        }
    }
        
    public static void MoveItems(IEnumerable<Item> items, ushort amount, Item targetContainer)
        => MoveItems(items, amount, targetContainer.Id, null);
    public static void MoveItems(IEnumerable<Item> items, ushort amount, ObjectId targetContainerId)
        => MoveItems(items, amount, targetContainerId, null);
    
    public static void MoveItems(IEnumerable<Item> items, ushort amount, ObjectId targetContainerId, Location2D? location)
    {
        foreach (var item in items)
        {
            if (amount == 0)
                break;
    
            if (item.Amount <= amount)
            {
                UO.ClientPrint($"Moving item {item.Amount} {Specs.TranslateToName(item)}");
                TryMoveItem(item, targetContainerId, location);
                amount -= item.Amount;
            }
            else
            {
                UO.ClientPrint($"Moving item {amount} {Specs.TranslateToName(item)}");
                TryMoveItem(item, amount, targetContainerId, location);
                amount = 0;
            }
        }
    }
    
    public static void ClickAllItems()
    {
        UO.ClientPrint("Select container");
        var sourceContainer = UO.AskForItem();
        if (sourceContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
        
        foreach (var item in UO.Items.InContainer(sourceContainer))
        {
            UO.Click(item);
            UO.Wait(100);
        }
    }
    
    public static void MoveAllItems()
    {
        UO.ClientPrint("Select source container");
        var sourceContainer = UO.AskForItem();
        if (sourceContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
    
        UO.ClientPrint("Select target container");
        var targetContainer = UO.AskForItem();
        if (targetContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
        
        var items = UO.Items.InContainer(sourceContainer);
        MoveItems(items, targetContainer);
    }
    
    public static void MoveItemsBetweenContainers(ItemSpec spec)
    {
        UO.ClientPrint("Select source container");
        var sourceContainer = UO.AskForItem();
        if (sourceContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
    
        UO.ClientPrint("Select target container");
        var targetContainer = UO.AskForItem();
        if (targetContainer == null)
        {
            UO.ClientPrint("Targeting canelled");
            return;
        }
        
        var items = UO.Items.InContainer(sourceContainer).Matching(spec);
        MoveItems(items, targetContainer);
        
        UO.ClientPrint("Moving finished");
    }
    
    public static void MoveFood()
    {
        MoveItemsBetweenContainers(Specs.Food);
    }
    
    public static void MoveRegs()
    {
        MoveItemsBetweenContainers(new[] { Specs.Regs, Specs.Gem });
    }
    
    public static void CountItemsSame()
    {
        UO.ClientPrint("Select item to count");
        var item = UO.AskForItem();
        if (item == null)
        {
            UO.ClientPrint("Counting cancelled");
            return;
        }
        
        var container = item.ContainerId.HasValue ? UO.Items[item.ContainerId.Value] : null;
        
        int count = (container != null) ? UO.Items.OfType(item.Type).OfColor(item.Color).InContainer(container).Count()
            : UO.Items.OfType(item.Type).OfColor(item.Color).OnGround().Count();
            
        UO.ClientPrint(count.ToString());
    }
    
    public static void AmountItemsSame()
        => AmountItemsSame(src => UO.Items.OfType(src.Type).OfColor(src.Color));

    public static void AmountItemsSameType()
        => AmountItemsSame(src => UO.Items.OfType(src.Type));
    
    private static void AmountItemsSame(Func<Item, IEnumerable<Item>> filter)
    {
        UO.ClientPrint("Select item to count amount");
        var item = UO.AskForItem();
        if (item == null)
        {
            UO.ClientPrint("Counting cancelled");
            return;
        }
        
        var container = item.ContainerId.HasValue ? UO.Items[item.ContainerId.Value] : null;
        
        int amount = (container != null) ? filter(item).InContainer(container).Sum(i => i.Amount)
            : UO.Items.OfType(item.Type).OfColor(item.Color).OnGround().Sum(i => i.Amount);
            
        UO.ClientPrint(amount.ToString());
    }
    
    public static void MoveSameItems()
    {
        MoveSameItems((src) => UO.Items.OfType(src.Type).OfColor(src.Color));
    }
    
    public static void MoveSameTypeItems()
    {
        MoveSameItems((src) => UO.Items.OfType(src.Type));
    }
    
    private static void MoveSameItems(Func<Item, IEnumerable<Item>> filter)
    {
        UO.ClientPrint("Select item");
        var sourceItem = UO.AskForItem();
        if (sourceItem == null)
        {
            UO.ClientPrint("Cancelled");
            return;
        }
        
        UO.ClientPrint("Select target container");
        var targetContainer = UO.AskForItem();
        if (sourceItem == null)
        {
            UO.ClientPrint("Cancelled");
            return;
        }
    
        Item[] items = null;
        if (sourceItem.ContainerId.HasValue)
        {
            var sourceContainer = UO.Items[sourceItem.ContainerId.Value];
            if (sourceContainer != null)
                items = filter(sourceItem).InContainer(sourceContainer).ToArray();
        }
        
        if (items == null)
            items = UO.Items.OfType(sourceItem.Type).OfColor(sourceItem.Color).ToArray();
        
        MoveItems(items, targetContainer);
    }
    
    public static void PickupFromGround(ushort type)
    {
        PickupFromGround((ModelId)type);
    }

    public static void PickupFromGround(params ModelId[] type)
    {
        var itemsOnGround = UO.Items.OfType(type).OnGround();
        foreach (var item in itemsOnGround)
        {
            UO.Log($"Picking up {item.Type}");
            Pickup(item);
            UO.Wait(250);
        }
    }
    
    public static void ItemsAmountAll()
    {
        ItemsAmountAll(false);
    }

    public static void ItemsAmountSub()
    {
        ItemsAmountAll(true);
    }

    private static void ItemsAmountAll(bool sub)
    {
        UO.ClientPrint("Select container");
        var container = UO.AskForItem();
        if (container == null)
        {
            UO.ClientPrint("Cancelled");
            return;
        }
        
        Dictionary<string, int> amounts = new Dictionary<string, int>();
        ItemsAmountAll(container, amounts, sub);

        foreach (var am in amounts)
            UO.ClientPrint($"{am.Key}: {am.Value}");
    }
    
    private static void ItemsAmountAll(Item container, Dictionary<string, int> amounts, bool sub)
    {
        var items = UO.Items.InContainer(container);
        UO.Log($"Container: {container.Name}");
        
        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                UO.Click(item);
                UO.Wait(100);
            }
        }
        
        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Name))
            {
                if (amounts.TryGetValue(item.Name, out int amount))
                    amounts[item.Name] = amount + item.Amount;
                else
                    amounts[item.Name] = item.Amount;
            }
            else
            {
                UO.Log($"Warning: item without name {item}");
            }
            
            if (sub)
                ItemsAmountAll(item, amounts, sub);
        }
    }
    
    public static bool Reload(ObjectId sourceContainerId, ushort targetAmount, params ItemSpec[] typesToReload)
    {
        var sourceContainer = UO.Items[sourceContainerId];
        if (sourceContainer == null)
        {
            UO.ClientPrint($"Cannot find {sourceContainerId} container.");
            return false;
        }
        
        return Reload(sourceContainer, targetAmount, typesToReload);
    }
    
    public static bool Reload(ObjectId sourceContainerId, ObjectId targetContainerId, int targetAmount, params ItemSpec[] typesToReload)
    {
        var sourceContainer = UO.Items[sourceContainerId];
        if (sourceContainer == null)
        {
            UO.ClientPrint($"Cannot find source container {sourceContainerId}.");
            return false;
        }

        var targetContainer = UO.Items[targetContainerId];
        if (targetContainer == null)
        {
            UO.ClientPrint($"Cannot find target container {targetContainer}.");
            return false;
        }
        
        return Reload(sourceContainer, targetContainer, targetAmount, null, typesToReload);
    }

    public static bool Reload(Item sourceContainer, ushort targetAmount, params ItemSpec[] typesToReload)
        => Reload(sourceContainer, UO.Me.BackPack, targetAmount, null, typesToReload);

    public static bool Reload(Item sourceContainer, Item targetContainer, int targetAmount, Location2D? location, params ItemSpec[] typesToReload)
        => Reload(sourceContainer.Id, targetContainer.Id, targetAmount, location, typesToReload);
    
    public static bool Reload(ObjectId sourceContainerId, ObjectId targetContainerId, int targetAmount, Location2D? location, params ItemSpec[] typesToReload)
    {
        UO.Log("Reloading");
    
        var currentItemsAmount = UO.Items.InContainer(targetContainerId).Matching(typesToReload).Sum(i => i.Amount);
        if (currentItemsAmount >= targetAmount)
        {
            UO.Log(
                $"Current amount ({currentItemsAmount}) is higher than or equal to target amount ({targetAmount}), no reloading");
            return true;
        }
    
        var sourceItemsToReload = UO.Items.InContainer(sourceContainerId, false).Matching(typesToReload).ToArray();
        if (sourceItemsToReload.Length <= 0)
        {
            UO.Log($"No items to reload found in {sourceContainerId}");
            return false;
        }
    
        Items.MoveItems(sourceItemsToReload, (ushort)(targetAmount - currentItemsAmount), targetContainerId, location);
        
        UO.Wait(100);
        
        return true;
    }
}

UO.RegisterCommand("moveitems-same", Items.MoveSameItems);
UO.RegisterCommand("moveitems-sametype", Items.MoveSameTypeItems);
UO.RegisterCommand("moveitems-all", Items.MoveAllItems);
UO.RegisterCommand("clickitems-all", Items.ClickAllItems);
UO.RegisterCommand("moveregs", Items.MoveRegs);
UO.RegisterCommand("movefood", Items.MoveFood );
UO.RegisterCommand("itemscount-same", Items.CountItemsSame);
UO.RegisterCommand("itemsamount-same", Items.AmountItemsSame);
UO.RegisterCommand("itemsamount-sametype", Items.AmountItemsSameType);
UO.RegisterCommand("itemsamount-all", Items.ItemsAmountAll);
UO.RegisterCommand("itemsamount-sub", Items.ItemsAmountSub);
UO.RegisterCommand("batchmove", Items.BatchMoveAllCommand);