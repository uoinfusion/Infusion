#load "colors.csx"
#load "equip.csx"
#load "warehouse.csx"

using System;
using System.Linq;
using System.Collections.Generic;

public class EquipSet
{
    public class StoredEquipment
    {
        public Layer Layer { get; set; }
        public ObjectId Id { get; set; }
        public ObjectId? ContainerId { get; set; } 
        
        public bool HasContainer => ContainerId.HasValue;
        
        public StoredEquipment(ObjectId id, Layer layer)
        {
            Id = id;
            Layer = layer;
            ContainerId = null;
        }
        
        public void Dress()
        {
            var item = UO.Items[Id];            
            
            if (item != null)
            {
                if (item.Layer.HasValue)
                {
                    UO.Log($"{Specs.TranslateToName(item)} already dressed up");
                    return;
                }

                UO.Log($"Dressing up {Specs.TranslateToName(item)}");
                ContainerId = item.ContainerId;
            }
            else
                UO.Log($"Dressing up {Id}");

            UO.DragItem(Id);
            UO.Wear(Id, Layer);
        }
                
        public void Undress(ObjectId defaultContainerId, bool forceDefault = false)
        {
            var item = UO.Items[Id];
            if (item == null)
                UO.ClientPrint($"Cannot find {Id}");
            if (!item.Layer.HasValue)
                return;
                
            var targetContainerId = (forceDefault) ? defaultContainerId : ContainerId ?? defaultContainerId;
                        
            UO.Log($"Undressing {Specs.TranslateToName(item)} -> {targetContainerId}");
            if (!Items.TryMoveItem(item, targetContainerId))
                UO.ClientPrint($"Cannot move {Specs.TranslateToName(item)}");        
        }
    }

    public static readonly Layer[] AllEquipLayers = new[]
    {
        Layer.Arms, Layer.Bracelet, Layer.Earrings, Layer.Gloves,
        Layer.Helm, Layer.Neck, Layer.OneHandedWeapon, Layer.TwoHandedWeapon,
        Layer.Pants, Layer.Ring, Layer.Shoes, Layer.Torso, Layer.Back, Layer.TorsoOuter,
        Layer.TorsoMiddle
    };
    
    public static Dictionary<string, EquipSet> EquipmentSets { get; set; }
        = new Dictionary<string, EquipSet>();

    public StoredEquipment[] Equips { get; set;}
       
    public void Dress()
    {
        foreach (var equip in Equips)
        {
            equip.Dress();
            UO.Wait(25);
        }
    }
    
    public void UndressTo()
    {
        var containerId = Common.AskForContainer("Select container to store equipment")?.Id;
        
        if (!containerId.HasValue)
        {
            UO.ClientPrint("Undress cancelled.");
            return;
        }
        
        Undress(containerId.Value, true);
    }
    
    public void Undress()
    {
        var equipWithContainer = Equips.Where(x => x.HasContainer).FirstOrDefault();
        var defaultContainerId = equipWithContainer?.ContainerId;
        
        if (!defaultContainerId.HasValue)
            defaultContainerId = Common.AskForContainer("Select container to store equipment")?.Id;

        if (!defaultContainerId.HasValue)
            defaultContainerId = UO.Me.BackPack.Id;

        Undress(defaultContainerId.Value, false);
    }
    
    public void Undress(uint defaultContainerId, bool forceDefault)
    {
        foreach (var equip in Equips)
        {
            equip.Undress(defaultContainerId, forceDefault);
        }
    }
    
    public static void CreateSet(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            UO.ClientPrint("Specify equip set name.");
            return;
        }
    
        var equipment = new List<StoredEquipment>();
        
        foreach (var layer in AllEquipLayers)
        {
            var item = UO.Items.OnLayer(layer).FirstOrDefault(); 
            if (item != null)
                equipment.Add(new StoredEquipment(item.Id, layer));
        }
        
        EquipmentSets[name] = new EquipSet()
        {
            Equips = equipment.ToArray(),
        };
        
        UO.ClientPrint($"Set '{name}' created.");
    }

    private static EquipSet GetEquipSet(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            UO.ClientPrint("Equip set name not specified.");
            return null;
        }
        
        if (!EquipmentSets.TryGetValue(name, out var equip))
        {
            UO.ClientPrint($"Unknown equip set '{name}'.");
            ListEquipSets();
            return null;
        }
        
        return equip;
    }
    
    public static void DressEquipSet(string name)
    {
        var equipSet = GetEquipSet(name);
        if (equipSet == null)
            return;
        
        UndressEquipSetAll();

        var closedContainerIds = equipSet.Equips
            .Where(x => x.HasContainer && UO.Items[x.Id] == null)
            .Select(x => x.ContainerId.Value)
            .Distinct();
        
        UO.ClientPrint($"Dressing up set {name}");
        
        equipSet.Dress();
    }

    public static void UndressEquipSetAll()
    {
        foreach (var set in EquipmentSets.Values)
        {
            set.Undress();
        }
    }

    public static void UndressEquipSet(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            UndressEquipSetAll();
            return;
        }
        
        var equipSet = GetEquipSet(name);
        if (equipSet == null)
            return;
        
        UO.ClientPrint($"Undressing set {name}");
        equipSet.Undress();
    }
    
    public static void UndressEquipSetTo(string name)
    {
        var equipSet = GetEquipSet(name);
        if (equipSet == null)
            return;

        UO.ClientPrint($"Undressing set {name}");        
        equipSet.UndressTo();
    }
    
    public static void RemoveEquipSet(string name)
    {
        var equipSet = GetEquipSet(name);
        if (equipSet == null)
            return;

        EquipmentSets.Remove(name);
        UO.ClientPrint($"Equipment set '{name}' removed.'");
    }
    
    public static void ListEquipSets()
    {
        if (!EquipmentSets.Any())
        {
            UO.ClientPrint("No equipment sets defined.");
            return;
        }
        
        UO.ClientPrint("Equipment set names:");
        foreach (var name in EquipmentSets.Keys)
        {
            UO.ClientPrint(name);
        }
    }
}

UO.Config.Register(() => EquipSet.EquipmentSets);
UO.RegisterCommand("equip-create", EquipSet.CreateSet);
UO.RegisterCommand("equip-dress", EquipSet.DressEquipSet);
UO.RegisterCommand("equip-undress", EquipSet.UndressEquipSet);
UO.RegisterCommand("equip-undress-to", EquipSet.UndressEquipSetTo);
UO.RegisterCommand("equip-list", EquipSet.ListEquipSets);
UO.RegisterCommand("equip-remove", EquipSet.RemoveEquipSet);
