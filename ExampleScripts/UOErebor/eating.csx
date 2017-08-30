#load "Specs.csx"

using System.Linq;

public static class Eating
{
    public static void EatFull()
    {
        UO.Log("Eating");
        var hasAnyFood = UO.Items.Matching(Specs.Food).InContainer(UO.Me.BackPack).Any();

        if (!hasAnyFood)
        {
            UO.Alert("Cannot find any food, I will starve and die soon!");
            return;
        }

        while (UO.TryUse(Specs.Food))
        {
            bool done = false;
            UO.Journal
                .When("You eat some", () => done = false)
                .When("You are stuffed!", "You are simply too full",
                    "You can't think of a way to use that item", () => done = true)
                .WaitAny();

            if (done)
                break;

            UO.Wait(250);
        }

        UO.Log("Eating finished");
    }
}

UO.RegisterCommand("eat-full", Eating.EatFull);
