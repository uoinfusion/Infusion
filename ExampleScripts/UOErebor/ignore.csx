#load "Specs.csx"

using System.Linq;
using System.Collections.Generic;

public class IgnoredItems
{
    private static object alreadyLootedItemsLock = new object();
    private static Dictionary<ObjectId, Item> alreadyLootedItems = new Dictionary<ObjectId, Item>();

    public static ScriptTrace Trace { get; } = UO.Trace.Create();
        
    public void Ignore(Item item)
    {
        lock (alreadyLootedItemsLock)
        {
            alreadyLootedItems[item.Id] = item;
        }
    }

    public bool IsIgnored(Item testedItem)
    {
        lock (alreadyLootedItemsLock)
        {
            if (alreadyLootedItems.TryGetValue(testedItem.Id, out Item ignoredItem))
            {
                // The problem is that Sphere recycles item IDs. It means that if you
                // have one ID it could mean two different items. It is painful especially
                // for corpses. There is the trick for corpses: Corpse cannot move, so if
                // we have the same ID but different location then it means an item with
                // a recycled ID.
                // Let's suppose that item type cannot change as well (this is definitivelly not
                // true for mobiles, because players have 0x0190 or 0x0191 and their ghosts 0x0192.             
                if (testedItem.Type != ignoredItem.Type || 
                    (Specs.Corpse.Matches(testedItem) && testedItem.Location != ignoredItem.Location))
                {
                    // Now, we can remove the ignored item, because we know, that the
                    // id was recycled.
                    alreadyLootedItems.Remove(testedItem.Id);
                    Trace.Log($"Found item {testedItem} that recycles id of {ignoredItem}. " +
                        "Removing previously ignored item.");
                    return false;
                }

                return true;
            }

            return false;
        }
    }
}