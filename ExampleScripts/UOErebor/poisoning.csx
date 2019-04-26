#load "Specs.csx"
#load "colors.csx"

public static class Poisoning
{
    public static void PoisonWeaponInHands()
    {
        var poisonPotion = UO.Items.InBackPack().Matching(Specs.Poison).FirstOrDefault();
        if (poisonPotion == null)
        {
            UO.ClientPrint("Cannot find a poison potion in your backpack", UO.Me, Colors.Red);
            return;
        }
        
        var weaponInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault() 
            ?? UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
        if (weaponInHand == null)
        {
            UO.ClientPrint("Cannot find a weapon in your hands");
            return;
        }
        
        UO.WaitTargetObject(weaponInHand);
        UO.Use(poisonPotion);
    }
}

UO.RegisterCommand("poison-hands", Poisoning.PoisonWeaponInHands);