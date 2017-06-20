using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class JournalEntries : IEnumerable<JournalEntry>
    {
        private static readonly AutoResetEvent receivedAwaitedWordsEvent = new AutoResetEvent(false);
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

            receivedAwaitedWordsEvent.Reset();
            while (!receivedAwaitedWordsEvent.WaitOne(TimeSpan.FromSeconds(1)))
                Legacy.CheckCancellation();

            awaitingWords = new string[] {};
        }

        internal void AddMessage(JournalEntry entry)
        {
            journal = journal.Add(entry);

            if (awaitingWords.Any(w => entry.Message.Contains(w)))
                receivedAwaitedWordsEvent.Set();

            OnNewMessageReceived(entry);
        }

        public JournalAwaiter<T> When<T>(string[] awaitedWords, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, string awaitedWord5, Func<JournalEntry, T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);

        public JournalAwaiter<T> When<T>(string[] awaitedWords, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);

        public JournalAwaiter<T> When<T>(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, string awaitedWord5, Func<T> whenAction) =>
            new JournalAwaiter<T>(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);

        public JournalAwaiter When(string awaitedWord1, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, string awaitedWord5, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);

        public JournalAwaiter When(string[] awaitedWords, Action whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);

        public JournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);

        public JournalAwaiter When(string awaitedWord1, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4, string awaitedWord5, Action<JournalEntry> whenAction) =>
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);

        public event EventHandler<JournalEntry> NewMessageReceived;

        private void OnNewMessageReceived(JournalEntry newEntry)
        {
            NewMessageReceived?.Invoke(this, newEntry);
        }
    }
}