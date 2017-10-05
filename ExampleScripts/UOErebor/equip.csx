public static class Equip
{
    public static Equipment GetHand()
    {
        Layer itemInHandLayer;
        
        var itemInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault();
        if (itemInHand == null)
        {
            itemInHand = UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
            itemInHandLayer = Layer.TwoHandedWeapon;
        }
        else
        {
            itemInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault();
            itemInHandLayer = Layer.OneHandedWeapon;
        }
    
        if (itemInHand != null)
            return new Equipment(itemInHand.Id, itemInHandLayer);
        else
            return Equipment.None;
    }
    
    public static void Set(Equipment equipment)
    {
        if (equipment.Equals(Equipment.None))
        {
            UO.ClientPrint("No item to equip");
            return;
        }
        
        var item = UO.Items[equipment.Id];
        if (item == null)
        {
            UO.ClientPrint($"Cannot find item {equipment.Id}.");
            return;            
        }

        UO.TryWear(item, equipment.Layer);
        UO.Wait(100);
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

    public override string ToString() => $"{Id}, {Layer}";
}