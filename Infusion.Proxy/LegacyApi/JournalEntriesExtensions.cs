using System;
using System.Collections.Generic;
using System.Linq;

namespace Infusion.Proxy.LegacyApi
{
    public static class JournalEntriesExtensions
    {
        public static IEnumerable<JournalEntry> After(this IEnumerable<JournalEntry> entries, DateTime createdAfter)
            => entries.Where(e => e.Created > createdAfter);

        public static IEnumerable<JournalEntry> After(this IEnumerable<JournalEntry> entries, long entryId)
            => entries.Where(e => e.Id > entryId);

        public static IEnumerable<JournalEntry> ByAnyName(this IEnumerable<JournalEntry> entries, params string[] names)
            => entries.Where(e => names.Contains(e.Name.ToLower()));

        public static IEnumerable<JournalEntry> ContainsAnyWord(this IEnumerable<JournalEntry> entries, params string[] words)
            => entries.Where(e => words.Any(w => e.Message.ToLower().Contains(w.ToLower())));
    }
}
