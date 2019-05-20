using System;
using System.Collections.Generic;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi.Keywords
{
    public class MulKeywordSource : IKeywordSource
    {
        public IEnumerable<SpeechEntry> Entries => SpeechList.Entries;
    }
}
