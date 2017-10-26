namespace Infusion.LegacyApi.Events
{
    public struct QuestArrowEvent
    {
        public bool Active { get; }
        public Location2D Location { get; }

        public QuestArrowEvent(bool active, Location2D location)
        {
            Active = active;
            Location = location;
        }
    }
}