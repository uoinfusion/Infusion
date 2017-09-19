namespace Infusion.LegacyApi
{
    public struct QuestArrowArgs
    {
        public bool Active { get; }
        public Location2D Location { get; }

        public QuestArrowArgs(bool active, Location2D location)
        {
            Active = active;
            Location = location;
        }
    }
}