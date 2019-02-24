using System;
using System.Collections.Generic;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi.Tests.EventJournalTests
{
    internal class TestEventJournalSource : IEventJournalSource
    {
        private readonly EventJournalSource source = new EventJournalSource();
        public event EventHandler<IEvent> NewEventReceived
        {
            add { source.NewEventReceived += value; }
            remove { source.NewEventReceived -= value; }
        }

        public void Publish(IEvent ev)
        {
            source.Publish(ev);
        }

        public void NotifyAction()
            => source.NotifyAction();

        public IEnumerable<OrderedEvent> Events => source.Events;
        public EventId LastEventId => source.LastEventId;
        public EventId LastActionEventId => source.LastActionEventId;
        public int MaximumCapacity => 128;
        public void GatherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId)
        {
            foreach (var ev in Events)
            {
                if (ev.Id > maxEventId)
                    return;

                if (ev.Id >= minEventId)
                    targetCollection.Add(ev.Event);
            }
        }
    }
}
