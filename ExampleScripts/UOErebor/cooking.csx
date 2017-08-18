#load "Specs.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public static class Cooking
{
    // Create a "private" instance of journal for cooking. If you delete this journal it
    // doesn't affect either UO.Journal or other instances of GameJournal.
    // It means, that you don't need tricks like UO.SetJournalLine(number,text) in
    // Injection.
    private static GameJournal journal = UO.CreateJournal();

    public static void Cook(ModelId rawFoodType, string campfireTile)
    {
        var rawFood = UO.Items.OfType(rawFoodType).InContainer(UO.Me.BackPack).First();
    
        while (rawFood != null)
        {
            UO.Use(rawFood);
            UO.WaitForTarget();
            UO.TargetTile(campfireTile);
    
            journal.WaitAny("Jidlo neni pozivatelne", "Mmm, smells good");
            UO.Wait(500);
    
            rawFood = UO.Items.OfType(rawFoodType).InContainer(UO.Me.BackPack).First();
        }
    }
    
    public static void Cook(ModelId rawFoodType, Item cookingPlace)
    {
        var rawFood = UO.Items.OfType(rawFoodType).InContainer(UO.Me.BackPack).First();
    
        while (rawFood != null)
        {
            UO.Use(rawFood);
            UO.WaitForTarget();
            UO.Target(cookingPlace);
    
            journal.WaitAny("Jidlo neni pozivatelne", "Mmm, smells good");
            UO.Wait(500);
    
            rawFood = UO.Items.OfType(rawFoodType).InContainer(UO.Me.BackPack).First();
        }
    }
    
    public static void Cook()
    {
        var cookingPlace = UO.Items.Matching(Specs.CookingPlaces).OnGround().MaxDistance(2).First();
    
        string cookingPlaceTile = null;
        if (cookingPlace == null)
        {
            UO.ClientPrint("Cooking place not found, specify a cooking place tile");
            cookingPlaceTile = UO.Info();
            if (cookingPlaceTile == null)
            {
                UO.ClientPrint("No cooking place tile found");
                return;
            }
        }
    
        var rawFood = UO.Items.Matching(Specs.RawFood).InContainer(UO.Me.BackPack).First();
        while (rawFood != null)
        {
            if (cookingPlace == null)
                Cook(rawFood.Type, cookingPlaceTile);
            else
                Cook(rawFood.Type, cookingPlace);
            rawFood = UO.Items.Matching(Specs.RawFood).InContainer(UO.Me.BackPack).First();
        }
    }
}

// Registering a ,cook command which executes Cook method without parameters.
UO.RegisterCommand("cook", () => Cooking.Cook());