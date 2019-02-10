#load "Specs.csx"

using System.Linq;
using System.Collections.Generic;

public class IgnoredItems
{
    private object ignoredItemsLock = new object();
    private  HashSet<ObjectId> alreadyLootedItems = new HashSet<ObjectId>();

    public static ScriptTrace Trace { get; } = UO.Trace.Create();
        
    public void Ignore(Item item) => Ignore(item.Id);
    public void Ignore(ObjectId id)
    {
        lock (ignoredItemsLock)
        {
            alreadyLootedItems.Add(id);
        }    
    }

    public bool IsIgnored(Item testedItem) => IsIgnored(testedItem.Id);
    public bool IsIgnored(ObjectId id)
    {
        lock (ignoredItemsLock)
        {
            return alreadyLootedItems.Contains(id);
        }
    }
}