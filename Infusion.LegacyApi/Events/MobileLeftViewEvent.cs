namespace Infusion.LegacyApi.Events
{
    public sealed class MobileLeftViewEvent : IEvent
    {
        internal MobileLeftViewEvent(Mobile mobile)
        {
            Mobile = mobile;
        }

        public Mobile Mobile { get; }
    }
}