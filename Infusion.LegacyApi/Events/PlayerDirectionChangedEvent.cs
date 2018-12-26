namespace Infusion.LegacyApi.Events
{
    public sealed class PlayerDirectionChangedEvent : IEvent
    {
        public Direction NewDirection { get; }
        public Direction OldDirection { get; }

        public PlayerDirectionChangedEvent(Direction newDirection, Direction oldDirection)
        {
            NewDirection = newDirection;
            OldDirection = oldDirection;
        }
    }
}
