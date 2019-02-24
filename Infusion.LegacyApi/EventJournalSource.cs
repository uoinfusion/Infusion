using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    internal class EventJournalSource : IEventJournalSource
    {
        private const int MaxEventCount = 2048;

        private readonly LinkedList<OrderedEvent> events = new LinkedList<OrderedEvent>();
        private readonly object sourceLock = new object();
        private int counter;
        public event EventHandler<OrderedEvent> NewEventReceived;

        public void Publish(IEvent ev)
        {
            OrderedEvent orderedEvent;
            lock (sourceLock)
            {
                var id = GenerateId();
                orderedEvent = new OrderedEvent(id, ev);
                events.AddLast(orderedEvent);
                LastEventId = id;
                counter++;

                if (counter >= MaxEventCount)
                {
                    events.RemoveFirst();
                    counter--;
                }
            }

            NewEventReceived?.Invoke(this, orderedEvent);
        }

        // ReSharper disable once InconsistentlySynchronizedField
        public IEnumerable<OrderedEvent> Events => events;

        public EventId LastEventId { get; private set; }
        public EventId LastActionEventId { get; private set; }
        public int MaximumCapacity => MaxEventCount;

        public void GatherEvents(ICollection<OrderedEvent> targetCollection, EventId minEventId, EventId maxEventId)
        {
            lock (sourceLock)
            {
                foreach (var ev in events)
                {
                    if (ev.Id > maxEventId)
                        return;

                    if (ev.Id > minEventId)
                        targetCollection.Add(ev);
                }
            }
        }

        public void NotifyAction()
        {
            LastActionEventId = LastEventId;
        }

        private EventId GenerateId() =>
            new EventId(Interlocked.Increment(ref counter));
    }
}