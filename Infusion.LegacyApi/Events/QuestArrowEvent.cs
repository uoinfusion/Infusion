namespace Infusion.LegacyApi.Events
{
    public sealed class QuestArrowEvent : IEvent
    {
        public bool Active { get; }
        public Location2D Location { get; }

        internal QuestArrowEvent(bool active, Location2D location)
        {
            Active = active;
            Location = location;
        }
    }
}