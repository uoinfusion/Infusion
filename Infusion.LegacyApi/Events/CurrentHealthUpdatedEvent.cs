namespace Infusion.LegacyApi.Events
{
    public struct CurrentHealthUpdatedEvent
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