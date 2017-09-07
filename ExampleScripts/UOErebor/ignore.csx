#load "Specs.csx"

using System.Linq;
using System.Collections.Generic;

public class IgnoredItems
{
    private static object alreadyLootedItemsLock = new object();
    private static Dictionary<ObjectId, Item> alreadyLootedItems = new Dictionary<ObjectId, Item>();
    
    public IgnoredItems()
    {
        UO.Events.ItemEnteredView += OnItemEnteredView;
    }
    
    private void OnItemEnteredView(object sender, ItemEnteredViewArgs args)
    {
        lock (alreadyLootedItemsLock)
        {
            if (alreadyLootedItems.TryGetValue(args.NewItem.Id, out Item item) &&
                ((Specs.Corpse.Matches(item) && item.Location != args.NewItem.Location) || item.Type != args.NewItem.Type))
            {
                // TODO: log as a debug message
                UO.ClientPrint($"Warning: removing ignored item, current item: {item}, new item: {args.NewItem}");
                alreadyLootedItems.Remove(args.NewItem.Id);
            }
        }
    }
    
    public void Ignore(Item item)
    {
        lock (alreadyLootedItemsLock)
        {
            alreadyLootedItems[item.Id] = item;
        }
    }

    public bool IsIgnored(Item item)
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
}