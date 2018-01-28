#load "colors.csx"
#load "Specs.csx"
#load "equip.csx"

public static class Healing
{
    public static void BandageFull()
    {
        while (UO.Me.CurrentHealth < UO.Me.MaxHealth)
        {
            if (!BandageSelf())
            {
                UO.Log("Cannot bandage self");
                return;
            }
            
            UO.Wait(100);
        }
        
        UO.Log("You are in a good health");
    }
    
    public static bool ReEquip { get; set; } = true;
    
    public static bool BandageSelf()
    {
        var weapon = Equip.GetHand();
    
        if (!UO.TryUse(Specs.Bandage))
        {
            UO.ClientPrint("No bandages", UO.Me, Colors.Red);
            return false;
        }
        
        try
        {
            UO.WaitTargetObject(UO.Me);
            
            bool result = true;
            UO.Journal
                .When("You are frozen and can not move.", () => result = false)
                .When("byl uspesne osetren", "Leceni se ti nepovedlo.", "neni zranen.", () => result = true)
                .WaitAny();

            if (ReEquip)
                Equip.Set(weapon);
                
            return result;
        }
        finally
        {
            UO.ClearTargetObject();
        }        
    }    
}

UO.RegisterCommand("bandage-full", Healing.BandageFull);
UO.RegisterCommand("bandage-self", () => Healing.BandageSelf());
