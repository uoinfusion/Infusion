using BenchmarkDotNet.Attributes;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi.Benchmarks
{
    [MemoryDiagnoser]
    public class EventJournal
    {
        private readonly EventJournalSource fullSource;
        private readonly LegacyApi.EventJournal fullJournal;
        private readonly EventJournalSource emptySource;
        private readonly LegacyApi.EventJournal emptyJournal;

        public EventJournal()
        {
            emptySource = new EventJournalSource();
            emptyJournal = new LegacyApi.EventJournal(emptySource);

            fullSource = new EventJournalSource();
            fullJournal = new LegacyApi.EventJournal(fullSource);

            for (int i = 0; i <= fullSource.MaximumCapacity + 1; i++)
                fullSource.Publish(new SpeechRequestedEvent("test"));
        }

        [Benchmark]
        public void EventPublicationWithFullJournal()
        {
            fullSource.Publish(new CommandRequestedEvent("test"));
        }

        [Benchmark]
        public void HandlingAllWithFullJournal()
        {
            fullJournal.When<SpeechRequestedEvent>(e => { })
                .All();
        }

        [Benchmark]
        public void EventPublicationWithEmptyJournal()
        {
            emptySource.Publish(new CommandRequestedEvent("test"));
        }

        [Benchmark]
        public void HandlingAllWithEmptyJournal()
        {
            emptyJournal.When<SpeechRequestedEvent>(e => { })
                .All();
        }
    }
}