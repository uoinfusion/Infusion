using System;
using System.Collections.Generic;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi.Keywords
{
    public interface IKeywordSource
    {
        IEnumerable<SpeechEntry> Entries { get; }
    }
}
