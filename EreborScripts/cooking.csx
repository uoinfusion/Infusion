#load "Scripts.csx"

using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Proxy;
using Infusion.Proxy.LegacyApi;
using static Infusion.Proxy.LegacyApi.Legacy;

void Cook(ModelId rawFoodType, string campfireTile)
{
    var rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();

    while (rawFood != null)
    {
        Use(rawFood);
        WaitForTarget();
        TargetTile(campfireTile);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        Scripts.Checks();

        rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();
    }
}

void Cook(ModelId rawFoodType, Item cookingPlace)
{
    var rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();

    while (rawFood != null)
    {
        Use(rawFood);
        WaitForTarget();
        Target(cookingPlace);

        WaitForJournal("Jidlo neni pozivatelne", "Mmm, smells good");
        Wait(500);

        Scripts.Checks();

        rawFood = Items.OfType(rawFoodType).InContainer(Me.BackPack).First();
    }
}

void Cook()
{
    var cookingPlace = Items.OfType(ItemTypes.CookingPlaces).OnGround().MaxDistance(Me.Location, 2).First();

    string cookingPlaceTile = null;
    if (cookingPlace == null)
    {
        Log("Cooking place not found, specify a cooking place tile");
        cookingPlaceTile = Info();
        if (cookingPlaceTile == null)
        {
            Log("No cooking place tile found");
            return;
        }
    }

    var rawFood = Items.OfType(ItemTypes.RawFood).InContainer(Me.BackPack).First();
    while (rawFood != null)
    {
        if (cookingPlace == null)
            Cook(rawFood.Type, cookingPlaceTile);
        else
            Cook(rawFood.Type, cookingPlace);
        rawFood = Items.OfType(ItemTypes.RawFood).InContainer(Me.BackPack).First();
    }
}

Legacy.CommandHandler.RegisterCommand(new Command("cook", () => Cook()));

