#load "items.csx"

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Equip
{
    public static ItemSpec NonusableEquip = new[] {Specs.GnarledStaff, Specs.BlackStaff};

    public static EquipmentSet GetHand()
    {
        List<Equipment> hands = new List<Equipment>();
        
        var oneHandedWeapon = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault();
        var twoHandedWeapon = UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
        
        if (oneHandedWeapon == null && twoHandedWeapon == null)
        return EquipmentSet.Empty;
        
        if (oneHandedWeapon != null)
            hands.Add(new Equipment(oneHandedWeapon.Id, Layer.OneHandedWeapon));
        if (twoHandedWeapon != null)
            hands.Add(new Equipment(twoHandedWeapon.Id, Layer.TwoHandedWeapon));
            
        return new EquipmentSet(hands);
    }

    public static void Set(EquipmentSet equipmentSet, TimeSpan? timeout = null)
    {
        if (!equipmentSet.Any())
        {
            UO.ClientPrint("Equipment set is empty, no item to equip.");
            return;
        }

        foreach (var equipment in equipmentSet)
        {
            var item = UO.Items[equipment.Id];
    
            if (item == null)
            {
                UO.ClientPrint($"Cannot find item {equipment.Id}.");
                return;
            }
            
            if (item.Layer != equipment.Layer)
            {
                // doubleclick is faster and safer, but cannot be used for staffs,
                // doubleclick stores all mana into the staff - so it has to be avoided
                if (NonusableEquip.Matches(item))
                {
                    UO.DragItem(item);
                    UO.Wear(item, equipment.Layer);
                }
                else
                {
                    UO.Use(item);
                }                    
            }
        }
    }
    
    public static void UndressJewelry()
    {
        Undress(Layer.Bracelet);
        Undress(Layer.Earrings);
        Undress(Layer.Neck);
        Undress(Layer.Ring);
    }
    
    public static void Undress(Layer layer)
    {
        foreach (var item in UO.Items.OnLayer(layer))
        {
            if (Items.Drag(item))
                UO.DropItem(item, UO.Me.BackPack);
        }
    }
}

public class EquipmentSet : IEnumerable<Equipment>
{
    public readonly static EquipmentSet Empty = new EquipmentSet(Array.Empty<Equipment>());

    private readonly IEnumerable<Equipment> equips;

    public EquipmentSet(IEnumerable<Equipment> equips)
    {
        this.equips = equips.ToArray();
    }
    
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    public IEnumerator<Equipment> GetEnumerator() => equips.GetEnumerator();

    public override string ToString()
    {
        if (!equips.Any())
            return "empty EquipmentSet";
            
        var builder = new StringBuilder();
        bool empty = true;
        foreach (var set in equips)
        {
            if (!empty)
                builder.Append(';');
            builder.Append(set.ToString());
            empty = false;
        }
        
        return builder.ToString();
    }
}

public struct Equipment
{
    public static readonly Equipment None = new Equipment((ObjectId)0, Layer.Arms);

    public Equipment(ObjectId id, Layer layer)
    {
        Id = id;
        Layer = layer;
    }

    public ObjectId Id { get; }
    public Layer Layer { get; }

    public override string ToString() => $"{Id} ({Layer})";
}

UO.RegisterCommand("undress-jewelry", Equip.UndressJewelry);
UO.RegisterCommand("undress-ring", () => Equip.Undress(Layer.Ring));
UO.RegisterCommand("undress-earrings", () => Equip.Undress(Layer.Earrings));