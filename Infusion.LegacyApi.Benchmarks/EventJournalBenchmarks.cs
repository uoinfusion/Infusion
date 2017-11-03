using BenchmarkDotNet.Attributes;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi.Benchmarks
{
    [MemoryDiagnoser]
    public class EventJournalBenchmarks
    {
        private readonly EventJournal emptyJournal;
        private readonly EventJournalSource emptySource;
        private readonly EventJournal fullJournal;
        private readonly EventJournalSource fullSource;

        public EventJournalBenchmarks()
        {
            emptySource = new EventJournalSource();
            emptyJournal = new EventJournal(emptySource);

            fullSource = new EventJournalSource();
            fullJournal = new EventJournal(fullSource);

            for (var i = 0; i <= fullSource.MaximumCapacity + 1; i++)
                fullSource.Publish(new SpeechRequestedEvent("test"));
        }

        [Benchmark]
        public void EventPublicationWithFullJournal()
        {
            fullSource.Publish(new CommandRequestedEvent("test"));
        }

        [Benchmark]
        public void HandlingAllWithFullJournal_NoneEventsHandled()
        {
            fullJournal.When<DialogBoxOpenedEvent>(e => { })
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