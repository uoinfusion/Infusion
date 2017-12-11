using System;
using System.Collections.Generic;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    internal class TestEventJournalSource : IEventJournalSource
    {
        private readonly EventJournalSource source = new EventJournalSource();
        public event EventHandler<IEvent> NewEventReceived;

        public void Publish(IEvent ev)
        {
            source.Publish(ev);
        }

        public IEnumerable<OrderedEvent> Events => source.Events;
        public EventId LastEventId => source.LastEventId;
        public int MaximumCapacity => 128;
        public void GetherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId)
        {
            foreach (var ev in Events)
            {
                if (ev.Id > maxEventId)
                    return;

                if (ev.Id >= minEventId)
                    targetCollection.Add(ev.Event);
            }
        }

        public void SignalEventReceived(IEvent ev)
        {
            NewEventReceived?.Invoke(this, ev);
        }
    }
}
