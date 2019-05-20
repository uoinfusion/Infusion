using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi.Keywords
{
    public sealed class MemoryKeywordSource : IKeywordSource
    {
        private readonly List<SpeechEntry> speechEntries = new List<SpeechEntry>();

        public MemoryKeywordSource()
        {
        }

        public void Add(params SpeechEntry[] entries) => speechEntries.AddRange(entries);

        public IEnumerable<SpeechEntry> Entries => speechEntries;
    }
}
