#load "Specs.csx"
#load "walking.csx"

public static class SheepShaving
{
    public static void ShaveSheeps()
    {
        var previousItemInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault() ?? UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
        var sheep = UO.Mobiles.Matching(Specs.Sheep).OrderByDistance().FirstOrDefault();
        while (sheep != null)
        {
            while (sheep != null && sheep.GetDistance(UO.Me.Location) > 1)
            {
                Walking.StepToward(sheep);
                sheep = UO.Mobiles.Refresh(sheep);
            }
            
            if (sheep != null)
            {
                UO.Use(Specs.Knives);
                UO.WaitForTarget();
                UO.Target(sheep);
                UO.Journal.WaitAny("You put the piles of wool in your pack.", "You put the pile of wool in your pack",
                    "You can't think of a way to use that item.", "You must wait for the wool to grow back");
                UO.Wait(1000);
            }
            
            sheep = UO.Mobiles.Matching(Specs.Sheep).OrderByDistance().FirstOrDefault();
        }
        
        if (previousItemInHand != null)
        {
            var itemInHand = UO.Items.OnLayer(Layer.OneHandedWeapon).FirstOrDefault() ?? UO.Items.OnLayer(Layer.TwoHandedWeapon).FirstOrDefault();
            if (previousItemInHand.Id != itemInHand.Id)
            {
                UO.Wear(previousItemInHand, Layer.OneHandedWeapon);
                UO.Wait(100);
            }
        }
    }
}

UO.RegisterCommand("shavesheeps", SheepShaving.ShaveSheeps);
