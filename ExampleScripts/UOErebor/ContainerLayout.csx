#load "common.csx"

using System.Linq;
using System.Collections.Generic;

public class ContainerLayout
{
    public static ScriptTrace Trace { get; } = UO.Trace.Create();

    private Dictionary<ItemSpec, Location2D> slots = new Dictionary<ItemSpec, Location2D>();
    public int ColumnWidth { get; }
    public int RowHeight { get; }

    public int SlotGridColumnCount { get; }
    public int SlotGridRowCount { get; }
    public int ContainerWidth { get; }
    public int ContainerHeight { get; }
    public int LeftMargin { get; }
    public int TopMargin { get; }
    public string Name { get; }

    public static ContainerLayout CreateBackpackLayout(string name, int gridWidth, int gridHeight)
        => new ContainerLayout(name, gridWidth, gridHeight, 162, 130, 44, 65);

    public ContainerLayout(string name, int slotGridColumnCount, int slotGridRowCount, int containerWidth, int containerHeight, int leftMargin, int topMargin)
    {
        Name = name;
        SlotGridColumnCount = slotGridColumnCount;
        SlotGridRowCount = slotGridRowCount;
        ContainerWidth = containerWidth;
        ContainerHeight = containerHeight;
        
        ColumnWidth = ContainerWidth / SlotGridColumnCount;
        RowHeight = ContainerHeight / SlotGridRowCount;
        LeftMargin = leftMargin;
        TopMargin = topMargin;
    }
    
    public void SetSlot(ItemSpec spec, int x, int y)
    {
        slots[spec] = new Location2D(x, y);
    }
    
    public bool TryGetGridLocation(Item item, out Location2D location)
    {
        foreach (var slot in slots)
        {
            if (slot.Key.Matches(item))
            {
                location = slot.Value;
                return true;
            }
        }
        
        location = default(Location2D);
        return false;
    }

    public bool TryGetGridLocation(ItemSpec spec, out Location2D location)
    {
        foreach (var slot in slots)
        {
            if (spec.IsKindOf(slot.Key))
            {
                location = slot.Value;
                return true;
            }
        }
        
        location = default(Location2D);
        return false;
    }
    
    public bool TryGetContainerLocation(Item item, out Location2D location)
    {
        if (TryGetGridLocation(item, out Location2D gridLocation))
        {
            location = SlotToContainer(gridLocation);
            return true;
        }
    
        location = default(Location2D);
        return false;        
    }
    
    public bool TryGetContainerLocation(ItemSpec spec, out Location2D location)
    {
        if (TryGetGridLocation(spec, out Location2D gridLocation))
        {
            location = SlotToContainer(gridLocation);
            return true;
        }
    
        location = default(Location2D);
        return false;        
    }
    
    public void Sort()
    {
        var container = Common.AskForContainer($"Select container to apply {Name} layout on");
        if (container != null)
            Sort(container.Id);
    }
    
    public void Sort(ObjectId containerId)
    {
        var items = UO.Items.InContainer(containerId).ToArray();
        
        foreach (var item in items)
        {
            if (TryGetGridLocation(item, out Location2D slotLocation))
            {
                var containerLocation = SlotToContainer(slotLocation);
                Trace.Log($"slotLocation {slotLocation}, containerLocation {containerLocation}, item location {item.Location}");
                if (item.Location != containerLocation)
                {
                    UO.Log($"Moving {Specs.TranslateToName(item)}");
                    UO.DragItem(item);
                    UO.WaitForItemDragged(item.Id);
                    UO.DropItem(item.Id, containerId, containerLocation);
                }
            }
        }
    }
    
    public Location2D SlotToContainer(Location2D slotLocation)
        => new Location2D(LeftMargin + (slotLocation.X * ColumnWidth), TopMargin + (slotLocation.Y * RowHeight)); 
}
