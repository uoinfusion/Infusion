using InjectionScript.Interpretation;
using System.Collections.Generic;

namespace Infusion.LegacyApi.Injection
{
    public sealed class Journal
    {
        private readonly LinkedList<JournalEntry> journal = new LinkedList<JournalEntry>();
        private readonly object journalLock = new object();

        public int MaxEntries { get; }

        internal Journal(int maxEntries) => MaxEntries = maxEntries;

        internal void Add(JournalEntry entry)
        {
            lock (journalLock)
            {
                journal.AddLast(entry);
                while (journal.Count > MaxEntries)
                    journal.RemoveFirst();
            }
        }

        public int InJournal(string pattern)
        {
            var searchWords = pattern.Split('|');

            lock (journalLock)
            {
                foreach (var word in searchWords)
                {
                    var foundIndex = journal.Count;
                    foreach (var entry in journal)
                    {
                        if (GetText(entry).Contains(word))
                            return foundIndex;

                        foundIndex--;
                    }
                }
            }

            return 0;
        }

        public void DeleteJournal()
        {
            lock (journalLock)
            {
                journal.Clear();
            }
        }

        public string GetJournalText(int index)
        {
            lock (journalLock)
            {
                var i = journal.Count - 1;
                foreach (var entry in journal)
                {
                    if (i == index)
                        return GetText(entry);

                    i--;
                }
            }

            return string.Empty;
        }

        private string GetText(JournalEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Name) && string.IsNullOrEmpty(entry.Message))
                return string.Empty;

            return entry.Text;
        }

        public string JournalSerial(int index)
        {
            lock (journalLock)
            {
                var i = journal.Count - 1;
                foreach (var entry in journal)
                {
                    if (i == index)
                        return NumberConversions.Int2Hex((int)entry.SpeakerId);

                    i--;
                }
            }

            return "0x00000000";
        }

        public void SetJournalLine(int index)
        {
            lock (journalLock)
            {
                var i = journal.Count - 1;
                var node = journal.First;

                while (node != null)
                {
                    if (i == index)
                    {
                        node.Value = new JournalEntry(node.Value.Id, string.Empty, string.Empty,
                            node.Value.SpeakerId, node.Value.Type, node.Value.Color);
                        break;
                    }

                    i--;
                    node = node.Next;
                }
            }
        }
    }
}
