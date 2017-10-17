namespace Infusion.LegacyApi.Events
{
    public sealed class CurrentHealthUpdatedEvent : IEvent
    {
        internal CurrentHealthUpdatedEvent(Mobile updatedMobile, ushort oldHealth)
        {
            UpdatedMobile = updatedMobile;
            OldHealth = oldHealth;
        }

        public Mobile UpdatedMobile { get; }
        public ushort OldHealth { get; }
    }
}