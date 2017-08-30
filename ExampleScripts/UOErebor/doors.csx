#load "Specs.csx"

using System.Globalization;

public static class Doors
{
    public static void OpenSpecificDoorCommand(string doorType)
    {
        if (string.IsNullOrEmpty(doorType))
        {
            UO.ClientPrint("no door type specified");
            return;
        }
        
        if (!ushort.TryParse(doorType, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort doorTypeNumber))
        {
            UO.ClientPrint($"door type '{doorType}' is not a valid type");
            return;
        }
        
        var type = (ModelId)doorTypeNumber;
    
        UO.Say(".resync");
        UO.Wait(250);
        var nearestDoor = UO.Items
            .OfType(type)
            .MaxDistance(2)
            .OrderByDistance()
            .FirstOrDefault();
    
        if (nearestDoor != null)
            UO.Use(nearestDoor);
        
        UO.Wait(250);
    }

    public static void ToggleNearestDoor()
    {
        var nearestDoor = UO.Items.Matching(Specs.Door).MaxDistance(5).OrderByDistance().FirstOrDefault();
        if (nearestDoor != null)
            UO.Use(nearestDoor);
        else
            UO.ClientPrint("Cannot find doors");
    }

    public static void OpenNearestDoor()
    {
        var nearestDoor = UO.Items.Matching(Specs.ClosedDoor).MaxDistance(5).OrderByDistance().FirstOrDefault();
        if (nearestDoor != null)
            UO.Use(nearestDoor);
        else
            UO.ClientPrint("Cannot find doors");
    }

    public static void CloseNearestDoor()
    {
        var nearestDoor = UO.Items.Matching(Specs.OpenDoor).MaxDistance(5).OrderByDistance().FirstOrDefault();
        if (nearestDoor != null)
            UO.Use(nearestDoor);
        else
            UO.ClientPrint("Cannot find doors");
    }
}

UO.RegisterCommand("door-toggle", Doors.ToggleNearestDoor);
UO.RegisterCommand("door-close", Doors.CloseNearestDoor);
UO.RegisterCommand("door-open", Doors.OpenNearestDoor);
UO.RegisterCommand("openspecificdoor", Doors.OpenSpecificDoorCommand);
