using System;
using System.Collections.Generic;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    internal interface IEventJournalSource
    {
        IEnumerable<OrderedEvent> Events { get; }
        EventId LastEventId { get; }
        EventId LastActionEventId { get; }
        int MaximumCapacity { get; }
        event EventHandler<OrderedEvent> NewEventReceived;
        void Publish(IEvent ev);
        void NotifyAction();
        void GatherEvents(ICollection<OrderedEvent> targetCollection, EventId minEventId, EventId maxEventId);
    }
}