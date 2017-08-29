#load "Specs.csx"

using System.Collections.Generic;
using System.Linq;

public static class Items
{
    public static void Pickup(Item item)
    {
        UO.TryMoveItem(item, UO.Me.BackPack);
    }
    
    public static void MoveItems(IEnumerable<Item> items, Item targetContainer)
    {
        foreach (var item in items)
        {
            UO.TryMoveItem(item, targetContainer);
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
                UO.TryMoveItem(item, targetContainer);
                amount -= item.Amount;
            }
            else
            {
                UO.TryMoveItem(item, amount, targetContainer);
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
}

UO.RegisterCommand("moveitems-same", Items.MoveSameItems);
UO.RegisterCommand("moveitems-all", Items.MoveAllItems);
UO.RegisterCommand("moveregs", Items.MoveRegs);
UO.RegisterCommand("movefood", Items.MoveFood );
UO.RegisterCommand("itemscount-same", Items.CountItemsSame);
UO.RegisterCommand("itemsamount-same", Items.AmountItemsSame);
