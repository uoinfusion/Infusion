using System.Linq;

public static partial class Walking
{
    public static void WalkPath(Direction[] path)
    {
        foreach (var direction in path)
        {
            if (UO.Me.Direction != direction)
            {
                UO.Walk(direction, MovementType.Run);
                UO.Log($"Turning to {direction}");
            }

            UO.Log($"Walking to {direction}");
            UO.Walk(direction, MovementType.Run);
        }
    }
    
    private static bool StepToward(Location2D currentLocation, Location2D targetLocation)
    {
        var walkVector = (targetLocation - currentLocation).Normalize();
        if (walkVector != Vector.NullVector)
        {
            var movementType = UO.Me.CurrentStamina > UO.Me.MaxStamina / 10 ? MovementType.Run : MovementType.Walk;

            var direction = walkVector.ToDirection();
            return UO.Walk(direction, movementType);
        }
        
        return true;
    }

    public static bool StepToward(GameObject gameObject)
    {
        return StepToward(gameObject.Location);
    }

    public static bool StepToward(Location2D targetLocation)
    {
        return StepToward(UO.Me.Location, targetLocation);
    }

    public static void WalkTo(Location2D targetLocation)
    {
        while (UO.Me.Location != targetLocation)
        {
            StepToward(targetLocation);
        }
    }

    public static void WalkTo(ushort xloc, ushort yloc)
    {
        WalkTo(new Location2D(xloc, yloc));
    }
    
    public static void WalkToPort(int portX, int portY, int targetX, int targetY)
    {
        WalkToPort(new Location2D(portX, portY), new Location2D(targetX, targetY));
    }
    
    public static void WalkToPort(Location2D portLocation, Location2D targetLocation)
    {
        while (UO.Me.Location != targetLocation)
        {
            StepToward(portLocation);
        }
    }
    
    public static void WalkToPortCommand(string parameters)
    {
        if (string.IsNullOrEmpty(parameters))
            throw new System.InvalidOperationException($"Invalid parameters {parameters}. Coordinates of port and target location required.");
        
        var coordsText = parameters.Split(',').Select(x => x.Trim()).ToArray();
        if (coordsText.Length != 4)
            throw new System.InvalidOperationException($"Invalid parameters {parameters}. Coordinates of port and target location required.");
            
        var coords = coordsText.Select(x => int.Parse(x)).ToArray();
        
        WalkToPort(coords[0], coords[1], coords[2], coords[3]);
    }
}

UO.RegisterCommand("walkto-port", Walking.WalkToPortCommand);