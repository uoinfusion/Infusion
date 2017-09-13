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
}