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
        private readonly SpeechJournalSource emptySource;

        private readonly SpeechJournal fullJournal;
        private readonly SpeechJournalSource fullSource;

        public SpeechJournalBenchmarks()
        {
            emptySource = new SpeechJournalSource();
            emptyJournal = new SpeechJournal(emptySource);

            fullSource = new SpeechJournalSource();
            for (int i = 0; i <= SpeechJournalSource.MaxLength; i++)
                fullSource.AddMessage("name", "message", 0, 0, (Color)0);
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
            emptySource.AddMessage("name", "message", 0, 0, (Color)0);
        }

        [Benchmark]
        public void EventPublicationWithFullJournal()
        {
            fullSource.AddMessage("name", "message", 0, 0, (Color)0);
        }
    }
}
