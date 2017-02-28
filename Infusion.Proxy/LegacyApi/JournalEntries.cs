using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public class JournalEntries : IEnumerable<JournalEntry>
    {
        private static readonly AutoResetEvent ReceivedAwaitedWordsEvent = new AutoResetEvent(false);
        private static string[] awaitingWords = {};
        private ImmutableList<JournalEntry> journal = ImmutableList<JournalEntry>.Empty;

        public IEnumerator<JournalEntry> GetEnumerator() => journal.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => journal.GetEnumerator();

        public bool InJournal(params string[] words) => journal.Any(line => words.Any(w => line.Message.Contains(w)));

        public bool InJournal(DateTime createdAfter, params string[] words)
            => journal.Any(line => line.Created > createdAfter && words.Any(w => line.Message.Contains(w)));

        public void DeleteJournal()
        {
            journal = ImmutableList<JournalEntry>.Empty;
        }

        public void WaitForJournal(params string[] words)
        {
            awaitingWords = words;

            ReceivedAwaitedWordsEvent.Reset();
            while (!ReceivedAwaitedWordsEvent.WaitOne(TimeSpan.FromSeconds(1)))
                Legacy.CheckCancellation();

            awaitingWords = new string[] {};
        }

        internal void AddMessage(JournalEntry entry)
        {
            journal = journal.Add(entry);

            if (awaitingWords.Any(w => entry.Message.Contains(w)))
                ReceivedAwaitedWordsEvent.Set();

            OnNewMessageReceived(entry);
        }

        public event EventHandler<JournalEntry> NewMessageReceived;

        protected virtual void OnNewMessageReceived(JournalEntry newEntry)
        {
            NewMessageReceived?.Invoke(this, newEntry);
        }
    }
}