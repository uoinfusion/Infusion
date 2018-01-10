#load "colors.csx"
#load "Specs.csx"
#load "equip.csx"

public static class Healing
{
    public static void BandageFull()
    {
        while (UO.Me.CurrentHealth < UO.Me.MaxHealth)
        {
            BandageSelf();
            UO.Wait(100);
        }
        
        UO.Log("You are in a good health");
    }
    
    public static bool ReEquip { get; set; } = true;
    
    public static void BandageSelf()
    {
        var weapon = Equip.GetHand();
    
        if (!UO.TryUse(Specs.Bandage))
        {
            UO.ClientPrint("No bandages", UO.Me, Colors.Red);
            return;
        }
        
        UO.WaitTargetObject(UO.Me);
        
        UO.Journal.WaitAny("byl uspesne osetren", "Leceni se ti nepovedlo.", "neni zranen.");
        
        if (ReEquip)
            Equip.Set(weapon);
    }    
}

UO.RegisterCommand("bandage-full", Healing.BandageFull);
UO.RegisterCommand("bandage-self", Healing.BandageSelf);
