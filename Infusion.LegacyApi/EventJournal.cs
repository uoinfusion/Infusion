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
        private readonly Func<CancellationToken?> cancellationTokenProvider;
        private readonly EventId journalStartEventId;

        internal EventJournal(IEventJournalSource source, Func<CancellationToken?> cancellationTokenProvider = null)
        {
            this.source = source;
            this.cancellationTokenProvider = cancellationTokenProvider;
            journalStartEventId = source.LastEventId;
        }

        public EventJournalAwaiter When<T>(Action<T> whenAction) where T : IEvent
        {
            return new EventJournalAwaiter(source, cancellationTokenProvider).When(whenAction);
        }

        public IEnumerator<IEvent> GetEnumerator() => source.Events.Where(e => e.Id > journalStartEventId).Select(e => e.Event).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}