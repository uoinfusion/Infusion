using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public event EventHandler<IEvent> NewEventReceived;

        public void Publish(IEvent ev)
        {
            lock (sourceLock)
            {
                var id = GenerateId();
                events.AddLast(new OrderedEvent(id, ev));
                LastEventId = id;
                counter++;

                if (counter >= MaxEventCount)
                {
                    events.RemoveFirst();
                    counter--;
                }
            }

            NewEventReceived?.Invoke(this, ev);
        }

        // ReSharper disable once InconsistentlySynchronizedField
        public IEnumerable<OrderedEvent> Events => events;

        public EventId LastEventId { get; private set; }
        public int MaximumCapacity => MaxEventCount;

        public void GetherEvents(ICollection<IEvent> targetCollection, EventId minEventId, EventId maxEventId)
        {
            lock (sourceLock)
            {
                foreach (var ev in events)
                {
                    if (ev.Id > maxEventId)
                        return;

                    if (ev.Id > minEventId)
                        targetCollection.Add(ev.Event);
                }
            }
        }

        private EventId GenerateId() =>
            new EventId(Interlocked.Increment(ref counter));
    }
}