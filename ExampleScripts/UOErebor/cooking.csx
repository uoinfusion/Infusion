#load "Specs.csx"
#load "Scripts.csx"

using System;
using System.Collections.Generic;
using System.Linq;

public static class Cooking
{
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
    
            Scripts.Checks();
    
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
    
            Scripts.Checks();
    
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

UO.RegisterCommand("cook", () => Cooking.Cook());