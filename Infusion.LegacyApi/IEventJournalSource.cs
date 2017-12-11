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
        event EventHandler<IEvent> NewEventReceived;
        void Publish(IEvent ev);
        void NotifyAction();
        void GatherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId);
    }
}