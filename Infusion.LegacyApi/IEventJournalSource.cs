using System;
using System.Collections.Generic;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    internal interface IEventJournalSource
    {
        event EventHandler<IEvent> NewEventReceived;
        void Publish(IEvent ev);
        IEnumerable<OrderedEvent> Events { get; }
        EventId LastEventId { get; }
        int MaximumCapacity { get; }
        void GetherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId);
    }
}