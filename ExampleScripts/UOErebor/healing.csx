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
    
    public static void BandageSelf()
    {
        var weapon = Equip.GetHand();
    
        UO.Use(Specs.Bandage);
        UO.WaitTargetObject(UO.Me);
        
        UO.Journal.WaitAny("byl uspesne osetren", "Leceni se ti nepovedlo.", "neni zranen.");
        
        Equip.Set(weapon);
    }    
}

UO.RegisterCommand("bandage-full", Healing.BandageFull);
UO.RegisterCommand("bandage-self", Healing.BandageSelf);
