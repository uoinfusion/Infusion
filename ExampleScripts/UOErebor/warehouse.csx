#load "Specs.csx"
#load "items.csx"
#load "container.csx"
#load "ContainerLayout.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public class Warehouse
{
    private Dictionary<ItemSpec, IContainer> locations = new Dictionary<ItemSpec, IContainer>();
    
    public static Warehouse Global = new Warehouse();

    public void AddContainer(ItemSpec spec, IContainer container)
    {
        locations.Add(spec, container);
    }
    
    public IContainer GetContainer(ItemSpec requestedSpec)
    {
        if (TryGetContainer(requestedSpec, out IContainer container))
            return container;
        
        throw new InvalidOperationException($"No registered container for {Specs.TranslateToName(requestedSpec)} in your warehouse."); 
    }

    public bool TryGetContainer(ItemSpec requestedSpec, out IContainer container)
    {
        foreach (var locationPair in locations)
        {
            if (requestedSpec.IsKindOf(locationPair.Key))
            {
                container = locationPair.Value; 
                return true;
            }
        }
        
        container = null;
        return false; 
    }
    
    public IContainer GetContainer(Item item)
    {
        if (TryGetContainer(item, out IContainer container))
            return container;
        
        throw new InvalidOperationException($"No registered container for {Specs.TranslateToName(item)}"); 
    }

    public Item AskForContainer(ItemSpec spec, string prompt)
    {
        Item containerItem;
        if (Warehouse.Global.TryGetContainer(spec, out IContainer container))
        {
            container.Open();
            containerItem = container.Item;
        }
        else
            containerItem = Common.AskForContainer(prompt);
            
        return containerItem;
    }
    
    public bool TryGetContainer(Item item, out IContainer container)
    {
        foreach (var locationPair in locations)
        {
            if (locationPair.Key.Matches(item))
            {
                container = locationPair.Value;
                return true;
            }
        }
        
        container = null;
        return false;
    }
    
    public void Sort(ObjectId sourceContainerId)
    {
        var items = UO.Items.InContainer(sourceContainerId, false).ToArray();
        
        foreach (var item in items)
        {
            if (TryGetContainer(item, out IContainer container))
            {
                container.Open();
                Items.TryMoveItem(item, container.Id);
            }
        }
    }
    
    public void Reload(ObjectId targetContainerId, ItemSpec spec, int targetAmount)
        => Reload(targetContainerId, null, spec, targetAmount);

    public void Reload(ObjectId targetContainerId, ContainerLayout layout, ItemSpec spec,
        int targetAmount, bool forceLayout = false)
    {
        Common.OpenContainer(targetContainerId);

        var currentAmount = UO.Items.InContainer(targetContainerId)
            .Matching(spec)
            .Sum(x => x.Amount);
            
        if (currentAmount >= targetAmount)
        {
            UO.Log($"No need to reload {Specs.TranslateToName(spec)}, current amount {currentAmount} is sufficient");
            return;
        }
            
        var container = GetContainer(spec);
        container.Open();
        
        bool doLayout = forceLayout || !UO.Items.InContainer(targetContainerId, false).Matching(spec).Any();
        doLayout &= layout != null;         
        if (doLayout && layout.TryGetContainerLocation(spec, out Location2D location))
        {
            UO.ClientPrint("Choosing location");
            Items.Reload(container.Id, targetContainerId, targetAmount, location, spec);
        }
        else
        {
            Items.Reload(container.Id, targetContainerId, targetAmount, spec);
        }
    }
}