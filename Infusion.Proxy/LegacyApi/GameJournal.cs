using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class GameJournal : IEnumerable<JournalEntry>
    {
        private readonly JournalSource source;
        private DateTime journalStartTime;

        internal GameJournal(JournalSource source)
        {
            this.source = source;
            source.NewMessageReceived += OnNewMessageReceived;
        }

        public IEnumerator<JournalEntry> GetEnumerator()
        {
            return source.Where(line => line.Created >= journalStartTime).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return source.Where(line => line.Created >= journalStartTime).GetEnumerator();
        }

        public event EventHandler<JournalEntry> NewMessageReceived;

        private void OnNewMessageReceived(object sender, JournalEntry journalEntry)
        {
            NewMessageReceived?.Invoke(this, journalEntry);
        }

        public bool Contains(params string[] words)
        {
            return source.Where(line => line.Created >= journalStartTime)
                .Any(line => words.Any(w => line.Message.Contains(w)));
        }

        public bool Contains(DateTime createdAfter, params string[] words)
        {
            return source.Any(line => line.Created > createdAfter && line.Created > journalStartTime &&
                                      words.Any(w => line.Message.Contains(w)));
        }

        public void Delete()
        {
            journalStartTime = DateTime.UtcNow;
        }

        public void WaitAny(params string[] words)
        {
            new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(words, () => { })
                .WaitAny();
        }

        public JournalAwaiter When(string awaitedWord1, Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);
        }

        public JournalAwaiter When(string[] awaitedWords, Action whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);
        }

        public JournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWords, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3,
            Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, whenAction);
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action<JournalEntry> whenAction)
        {
            return new JournalAwaiter(() => Legacy.CancellationToken, this)
                .When(awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5, whenAction);
        }
    }
}