using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using Ultima;

namespace Infusion.LegacyApi.Injection
{
    public sealed class Journal
    {
        private const string clilocPrefix = "cliloc# 0x";
        private readonly LinkedList<JournalEntry> journal = new LinkedList<JournalEntry>();
        private readonly object journalLock = new object();
        private static readonly Lazy<StringList> clilocDictionary = new Lazy<StringList>(() => new StringList("ENU"));

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

        public int InJournal(string searchPhrase)
        {
            var searchPatterns = searchPhrase.Split('|');

            lock (journalLock)
            {
                foreach (var pattern in searchPatterns)
                {
                    string word = pattern;

                    if (pattern.StartsWith(clilocPrefix, StringComparison.OrdinalIgnoreCase) && pattern.Length > clilocPrefix.Length)
                    {
                        var messageIdText = pattern.Substring(clilocPrefix.Length).Trim();
                        var messageId = int.Parse(messageIdText, System.Globalization.NumberStyles.HexNumber) + 0x70000;
                        word = clilocDictionary.Value.GetString(messageId);
                    }

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

        internal string JournalColor(int index) => ProcessJournalIndex(index, GetColor, "0x0000");
        private string GetColor(JournalEntry entry) => entry.Color.ToString();

        internal string JournalSerial(int index) => ProcessJournalIndex(index, GetSerial, "0x00000000");
        private string GetSerial(JournalEntry entry) => entry.Id.ToString();

        internal string GetJournalText(int index) => ProcessJournalIndex(index, GetText, string.Empty);
        private string GetText(JournalEntry entry)
        {
            if (string.IsNullOrEmpty(entry.Name) && string.IsNullOrEmpty(entry.Message))
                return string.Empty;

            return entry.Text;
        }

        private string ProcessJournalIndex(int index, Func<JournalEntry, string> processFunc, string @default)
        {
            lock (journalLock)
            {
                var i = journal.Count - 1;
                foreach (var entry in journal)
                {
                    if (i == index)
                        return processFunc(entry);

                    i--;
                }
            }

            return @default;
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
