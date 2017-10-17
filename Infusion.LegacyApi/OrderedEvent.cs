using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    internal struct OrderedEvent
    {
        public OrderedEvent(EventId id, IEvent @event)
        {
            Id = id;
            Event = @event;
        }

        public EventId Id { get; }
        public IEvent Event { get; }


    }
}