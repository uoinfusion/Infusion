using System;
using System.Collections.Generic;
using Ultima;

namespace Infusion.LegacyApi.Injection
{
    public sealed class Journal
    {
        private class Entry
        {
            public string Message { get; set; }
            public string Name { get; set; }

            public ObjectId SpeakerId { get; set; }
            public Color Color { get; set; }
            public int Created { get; set; }

            public string Text => $"{Name ?? "no name"}: {Message}";
        }


        private const string clilocPrefix = "cliloc# 0x";
        private readonly LinkedList<Entry> journal = new LinkedList<Entry>();
        private readonly object journalLock = new object();
        private static readonly Lazy<StringList> clilocDictionary = new Lazy<StringList>(() => new StringList("ENU"));
        private readonly Func<int> provideNow;

        public int MaxEntries { get; }

        internal Journal(int maxEntries, Func<int> provideNow)
        {
            MaxEntries = maxEntries;
            this.provideNow = provideNow;
        }

        internal void Add(JournalEntry entry)
        {
            lock (journalLock)
            {
                journal.AddFirst(new Entry
                {
                    Color = entry.Color,
                    Created = provideNow(),
                    Message = entry.Message,
                    Name = entry.Name,
                    SpeakerId = entry.SpeakerId,
                });
                while (journal.Count > MaxEntries)
                    journal.RemoveLast();
            }
        }

        internal int InJournalBetweenTime(string searchPhrase, int startTime, int endTime, int limit)
        {
            var searchPatterns = searchPhrase.Split('|');

            lock (journalLock)
            {
                foreach (var pattern in searchPatterns)
                {
                    var word = pattern;

                    if (pattern.StartsWith(clilocPrefix, StringComparison.OrdinalIgnoreCase) && pattern.Length > clilocPrefix.Length)
                    {
                        var messageIdText = pattern.Substring(clilocPrefix.Length).Trim();
                        var messageId = int.Parse(messageIdText, System.Globalization.NumberStyles.HexNumber) + 0x70000;
                        word = clilocDictionary.Value.GetString(messageId);
                    }

                    var foundIndex = 1;
                    var entry = journal.First;
                    while (entry != null)
                    {
                        if ((startTime < 0 || entry.Value.Created >= startTime)
                            && (endTime < 0 || entry.Value.Created <= endTime)
                            && (limit < 0 || foundIndex <= limit)
                            && GetText(entry.Value).Contains(word))
                        {
                            return foundIndex;
                        }

                        entry = entry.Next;
                        foundIndex++;
                    }
                }
            }

            return 0;
        }

        public int InJournal(string searchPhrase)
            => InJournalBetweenTime(searchPhrase, -1, -1, -1);

        public void DeleteJournal()
        {
            lock (journalLock)
            {
                journal.Clear();
            }
        }

        public void DeleteJournal(string text)
        {
            lock (journalLock)
            {
                var node = journal.First;
                while (node != null)
                {
                    if (node.Value.Text.Contains(text))
                    {
                        node.Value.Message = null;
                        node.Value.Name = null;
                    }
                    node = node.Next;
                }
            }
        }

        internal string JournalColor(int index) => ProcessJournalIndex(index, GetColor, "0x0000");
        private string GetColor(Entry entry) => entry.Color.ToString();

        internal string JournalSerial(int index) => ProcessJournalIndex(index, GetSerial, "0x00000000");
        private string GetSerial(Entry entry) => entry.SpeakerId.ToString();

        internal string GetJournalText(int index) => ProcessJournalIndex(index, GetText, string.Empty);
        private string GetText(Entry entry)
        {
            if (string.IsNullOrEmpty(entry.Name) && string.IsNullOrEmpty(entry.Message))
                return string.Empty;

            return entry.Text;
        }

        private string ProcessJournalIndex(int index, Func<Entry, string> processFunc, string @default)
        {
            lock (journalLock)
            {
                var i = 0;
                var node = journal.First;
                while (node != null)
                {
                    if (i == index)
                        return processFunc(node.Value);

                    node = node.Next;
                    i++;
                }
            }

            return @default;
        }

        public void SetJournalLine(int index, string message)
        {
            lock (journalLock)
            {
                var i = 0;
                var node = journal.First;

                while (node != null)
                {
                    if (i == index)
                    {
                        if (string.IsNullOrEmpty(message))
                        {
                            node.Value.Message = null;
                            node.Value.Name = null;
                        }
                        else
                            node.Value.Message = message;

                        break;
                    }

                    i++;
                    node = node.Next;
                }
            }
        }
    }
}
