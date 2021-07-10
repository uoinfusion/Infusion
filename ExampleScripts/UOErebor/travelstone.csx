#load "Specs.csx"
#load "common.csx"

using Infusion.Commands;

public static class TravelStone
{
    // doesn't support destinations on the second page yet
    public static void TravelTo(string destination)
    {
        var travelStone = UO.Items.OnGround().MaxDistance(4).Matching(Specs.TravelStone).FirstOrDefault();
        if (travelStone != null)
        {
            UO.Use(travelStone);
            UO.WaitForGump();
            UO.LastGumpInfo();
            UO.SelectGumpButton(destination, Infusion.Gumps.GumpLabelPosition.Before);
        }
        else
        {
            UO.Console.Error("Cannot find travelstone.");
        }
    }
    
    public static void TravelToCommand(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            throw new CommandInvocationException("Destination name not specified.");
            
        TravelTo(parameters);
        Common.WaitForChangedLocation();
    }
}

UO.RegisterCommand("travelto", TravelStone.TravelToCommand);
