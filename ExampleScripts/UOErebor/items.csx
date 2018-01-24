#load "Specs.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public static class Items
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();
    private static EventJournal journal = UO.CreateEventJournal();

    public static string GetName(Item item)
        => string.IsNullOrEmpty(item.Name) ? Specs.TranslateToName(item) : item.Name;
    

    public static bool Drag(Item item)
        => Drag(item, 1);
    
    public static bool Drag(Item item, ushort amount)
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

    public static bool TryMoveItem(Item item, ObjectId targetContainerId)
        => TryMoveItem(item, item.Amount, targetContainerId);

    public static bool TryMoveItem(Item item, ushort amount, GameObject targetContainer)
        => TryMoveItem(item, amount, targetContainer.Id);
    
    public static bool TryMoveItem(Item item, ushort amount, ObjectId targetContainerId)
    {
        bool result = false;
    
        if (!Drag(item, amount))
            return false;
        
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
    
    public static void BatchedMove(int totalAmount, int batchSize)
    {
        UO.Log("Select item to move");
        var itemTemplate = UO.AskForItem();
        if (itemTemplate == null)
        {
            UO.Log("Moving cancelled");
            return;
        }
        
        UO.Log("Select target container");
        var targetContainer = UO.AskForItem();
        if (targetContainer == null)
        {
            UO.Log("Moving cancelled");
            return;
        }
        
        while (totalAmount > 0)
        {
            var itemsToMove = UO.Items.OfColor(itemTemplate.Color).OfType(itemTemplate.Type);
            if (itemTemplate.ContainerId.HasValue)
            {
                itemsToMove = itemsToMove.InContainer(itemTemplate.ContainerId.Value);
            }
            
            var itemsToMoveOnSamePosition = itemsToMove.Where(x => x.Location == itemTemplate.Location).ToArray();
            if (itemsToMoveOnSamePosition.Any())
                itemsToMove = itemsToMoveOnSamePosition;
            
            var itemToMove = itemsToMove.FirstOrDefault();
            if (itemToMove == null)
            {
                UO.Log("No item to move found");
                break;
            }
            var amount = batchSize > totalAmount ? totalAmount : batchSize;
            amount = amount > itemToMove.Amount ? itemToMove.Amount : amount;
            
            UO.Log($"Moving {amount}, {totalAmount} to move");
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
    {
        MoveItems(items, amount, targetContainer.Id);
    }
    
    public static void MoveItems(IEnumerable<Item> items, ushort amount, ObjectId targetContainerId)
    {
        foreach (var item in items)
        {
            if (amount == 0)
                break;
    
            UO.ClientPrint($"Moving item {Specs.TranslateToName(item)}");
            if (item.Amount <= amount)
            {
                TryMoveItem(item, targetContainerId);
                amount -= item.Amount;
            }
            else
            {
                TryMoveItem(item, amount, targetContainerId);
                amount = 0;
            }
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
    {
        UO.ClientPrint("Select item to count amount");
        var item = UO.AskForItem();
        if (item == null)
        {
            UO.ClientPrint("Counting cancelled");
            return;
        }
        
        var container = item.ContainerId.HasValue ? UO.Items[item.ContainerId.Value] : null;
        
        int amount = (container != null) ? UO.Items.OfType(item.Type).OfColor(item.Color).InContainer(container).Sum(i => i.Amount)
            : UO.Items.OfType(item.Type).OfColor(item.Color).OnGround().Sum(i => i.Amount);
            
        UO.ClientPrint(amount.ToString());
    
    }
    
    public static void MoveSameItems()
    {
        UO.ClientPrint("Select item");
        var sourceItem = UO.AskForItem();
        if (sourceItem == null)
        {
            UO.ClientPrint("MoveItems cancelled");
            return;
        }
        
        UO.ClientPrint("Select target container");
        var targetContainer = UO.AskForItem();
        if (sourceItem == null)
        {
            UO.ClientPrint("MoveItems cancelled");
            return;
        }
    
        Item[] items = null;
        if (sourceItem.ContainerId.HasValue)
        {
            var sourceContainer = UO.Items[sourceItem.ContainerId.Value];
            if (sourceContainer != null)
                items = UO.Items.InContainer(sourceContainer).OfType(sourceItem.Type).OfColor(sourceItem.Color).ToArray();
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

        string description = amounts
            .OrderBy(x => x.Key)
            .Select(x => $"{x.Key}: {x.Value}")
            .Aggregate((l, r) => l + "\n" + r);
        UO.Log(description);
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
}

UO.RegisterCommand("moveitems-same", Items.MoveSameItems);
UO.RegisterCommand("moveitems-all", Items.MoveAllItems);
UO.RegisterCommand("moveregs", Items.MoveRegs);
UO.RegisterCommand("movefood", Items.MoveFood );
UO.RegisterCommand("itemscount-same", Items.CountItemsSame);
UO.RegisterCommand("itemsamount-same", Items.AmountItemsSame);
UO.RegisterCommand("itemsamount-all", Items.ItemsAmountAll);
UO.RegisterCommand("itemsamount-sub", Items.ItemsAmountSub);

//var item = UO.Items[0x4008B909];
//Items.Wear(item, Layer.TwoHandedWeapon);