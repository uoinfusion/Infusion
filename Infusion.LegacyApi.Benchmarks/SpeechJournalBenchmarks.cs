using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Infusion.LegacyApi.Events;

namespace Infusion.LegacyApi.Benchmarks
{
    [MemoryDiagnoser]
    public class SpeechJournalBenchmarks
    {
        private readonly SpeechJournal emptyJournal;
        private readonly JournalSource emptySource;

        private readonly SpeechJournal fullJournal;
        private readonly JournalSource fullSource;

        public SpeechJournalBenchmarks()
        {
            emptySource = new JournalSource();
            emptyJournal = new SpeechJournal(emptySource);

            fullSource = new JournalSource();
            for (int i = 0; i <= JournalSource.MaxLength; i++)
                fullSource.AddMessage("name", "message", 0, 0);
            fullJournal = new SpeechJournal(fullSource);
        }

        [Benchmark]
        public void ContainsWithEmptyJournal()
        {
            emptyJournal.Contains("some other name");
        }

        [Benchmark]
        public void ContainsWithFullJournal()
        {
            fullJournal.Contains("something that is not really in journal");
        }

        [Benchmark]
        public void EventPublicationWithEmptyJournal()
        {
            emptySource.AddMessage("name", "message", 0, 0);
        }

        [Benchmark]
        public void EventPublicationWithFullJournal()
        {
            fullSource.AddMessage("name", "message", 0, 0);
        }
    }
}
