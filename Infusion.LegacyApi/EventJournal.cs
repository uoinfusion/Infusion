using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi
{
    public class EventJournal : IEnumerable<IEvent>
    {
        private readonly IEventJournalSource source;
        private readonly Cancellation cancellation;
        private readonly Func<TimeSpan?> defaultTimeout;

        public EventId JournalStartEventId { get; internal set; }
        public EventId LastEventId => source.LastEventId;

        internal AutoResetEvent AwaitingStarted { get; } = new AutoResetEvent(false);

        internal EventJournal(IEventJournalSource source, Cancellation cancellation = null, Func<TimeSpan?> defaultTimeout = null)
        {
            this.source = source;
            this.cancellation = cancellation;
            this.defaultTimeout = defaultTimeout;
            JournalStartEventId = source.LastEventId;
        }

        public EventJournalAwaiter When<T>(Action<T> whenAction) where T : IEvent
        {
            return new EventJournalAwaiter(source, cancellation, this, defaultTimeout).When(whenAction);
        }

        public EventJournalAwaiter When(string s1, Action whenAction)
        {
            return new EventJournalAwaiter(source, cancellation, this, defaultTimeout).When(s1, whenAction);
        }

        public EventJournalAwaiter When<T>(Func<T, bool> acceptEventPredicate, Action<T> whenAction) where T : IEvent
        {
            return new EventJournalAwaiter(source, cancellation, this, defaultTimeout).When(acceptEventPredicate, whenAction);
        }

        public IEnumerator<IEvent> GetEnumerator() => OrderedEvents.Select(e => e.Event).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        internal IEnumerable<OrderedEvent> OrderedEvents => source.Events.Where(e => e.Id > JournalStartEventId);

        public void Delete()
        {
            JournalStartEventId = source.LastEventId;
        }

        internal void GatherEvents(ICollection<OrderedEvent> targetCollection, EventId minEventId, EventId maxEventId)
            => source.GatherEvents(targetCollection, minEventId > JournalStartEventId ? minEventId : JournalStartEventId, maxEventId);

        public void NotifyWait(EventId eventId)
        {
            LastWaitEventId = eventId.Next();
        }

        public EventId LastWaitEventId { get; private set; }

        internal void GetherWaitEnyEvents(List<OrderedEvent> targetCollection)
        {
            if (LastWaitEventId > source.LastActionEventId)
                source.GatherEvents(targetCollection, LastWaitEventId, EventId.MaxValue);
            else
                source.GatherEvents(targetCollection, source.LastActionEventId, EventId.MaxValue);
        }
    }
}