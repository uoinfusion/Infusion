#load "warehouse.csx"

using System.Linq;
using System.Collections.Generic;

public class EquipSet
{
    public ObjectId[] ItemIds { get; }
    public Container Container { get; }
    
    public static void BuildConstructor()
    {
        var ids = new HashSet<ObjectId>();

        while (true)
        {
            UO.ClientPrint("Select item for equip or press esc");
            Item selectedItem = UO.AskForItem();
            if (selectedItem == null)
            {
                UO.ClientPrint("Equip build canceled");
                break;
            }
            
            ids.Add(selectedItem.Id);
        }
        
        var idsText = ids.Select(x => x.ToString()).Aggregate(string.Empty, (l, r) => l + "," + r);
        
        UO.Log($"var equipSet = new EquipSet(<put container here>{idsText});");
    }
    
    public EquipSet(Container container, params ObjectId[] itemIds)
    {
        this.ItemIds = itemIds;
        Container = container;
    }
    
    public void Equip()
    {
        UO.ClientPrint("Equiping set");
        Common.OpenContainer(Container.Id);
    
        foreach (var id in ItemIds)
        {
            var item = UO.Items[id];
            if (item == null)
            {
                UO.ClientPrint($"Cannot find item {id}"); 
                continue;
            }
            
            UO.Log($"Equiping {Specs.TranslateToName(item)}");
            UO.Use(item);
        }
    }
    
    public void Unequip()
    {
        UO.ClientPrint("Unequiping set");
        Common.OpenContainer(Container.Id);
        
        foreach (var id in ItemIds)
        {
            var item = UO.Items[id];
            if (item == null)
                UO.ClientPrint($"Cannot find {id}");

            if (!Container.Contains(item))
            {
                UO.Log($"Unequiping {Specs.TranslateToName(item)} -> {Container.Id}");
                if (!Items.TryMoveItem(item, Container.Id))
                    UO.ClientPrint($"Cannot move {Specs.TranslateToName(item)}");
            }
        }
    }
    
    public void Open() => Container.Open();
    
    public void Load()
    {
        Container.Open();
        Items.TryMoveItem(Container.Item, UO.Me.BackPack);
    }
    
    public void Unload()
    {
        if (!Container.InParentContainer)
        {
            Container.Parent.Open();
            Items.TryMoveItem(Container.Item, Container.Parent.Item);
        }
    }
}