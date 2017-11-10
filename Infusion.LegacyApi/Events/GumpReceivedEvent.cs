using Infusion.Gumps;

namespace Infusion.LegacyApi.Events
{
    public sealed class GumpReceivedEvent : IEvent
    {
        internal GumpReceivedEvent(Gump gump)
        {
            Gump = gump;
        }

        public Gump Gump { get; }
    }
}