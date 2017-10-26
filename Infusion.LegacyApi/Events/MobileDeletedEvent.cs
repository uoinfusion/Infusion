namespace Infusion.LegacyApi.Events
{
    public sealed class MobileDeletedEvent : IEvent
    {
        internal MobileDeletedEvent(Mobile mobile)
        {
            Mobile = mobile;
        }

        public Mobile Mobile { get; }
    }
}