namespace Infusion
{
    public enum DragResult
    {
        CannotLift = 0,
        OutOfRange = 1,
        OutOfSight = 2,
        BelongsToAnother = 3,
        AlreadyHoldingSomething = 4,
        Unspecified = 5,
        None = 253,
        Success = 254,
        Timeout = 255,
    }
}
