#load "Specs.csx"

public static class Healing
{
    public static void BandageFull()
    {
        while (UO.Me.CurrentHealth < UO.Me.MaxHealth)
        {
            UO.Use(Specs.Bandage);
            UO.WaitForTarget();
            UO.Target(UO.Me);
            
            UO.Journal.WaitAny("byl uspesne osetren", "Leceni se ti nepovedlo.", "neni zranen.");
            UO.Wait(100);
        }
        
        UO.Log("You are in a good health");
    }
}

UO.RegisterCommand("bandage-full", Healing.BandageFull);
