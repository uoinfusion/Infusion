using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public sealed class JournalAwaiter
    {
        private readonly EventWaitHandle entryReceivedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly JournalSource journalSource;
        private readonly GameJournal journal;
        private readonly Func<CancellationToken?> tokenProvider;

        private readonly Dictionary<string[], Action<JournalEntry>> whenActions =
            new Dictionary<string[], Action<JournalEntry>>();

        private Action<JournalEntry> receivedAction;
        private JournalEntry receivedJournalEntry;

        private Action timeoutAction;

        internal JournalAwaiter(Func<CancellationToken?> tokenProvider, JournalSource journalSource = null, GameJournal journal = null)
        {
            this.tokenProvider = tokenProvider;
            this.journalSource = journalSource;
            this.journal = journal;
        }

        internal void ReceiveJournalEntry(JournalEntry entry)
        {
            var keyValuePair =
                whenActions.FirstOrDefault(pair => pair.Key.Any(awaitedWord => entry.Text.Contains(awaitedWord)));
            if (keyValuePair.Key != null && keyValuePair.Value != null)
            {
                receivedAction = keyValuePair.Value;
                receivedJournalEntry = entry;

                journal?.NotifyWait();
                entryReceivedEvent.Set();
            }
        }

        public JournalAwaiter When(string awaitedWord1, Action whenAction)
        {
            whenActions[new[] {awaitedWord1}] = entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string[] awaitedWords, Action whenAction)
        {
            whenActions[awaitedWords] = entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2}] = entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3}] = entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4}] = entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5}] =
                entry => whenAction();

            return this;
        }

        public JournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction)
        {
            whenActions[awaitedWords] = whenAction;

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1}] = whenAction;

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2}] = whenAction;

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3,
            Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3}] = whenAction;

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4}] = whenAction;

            return this;
        }

        public JournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5}] = whenAction;

            return this;
        }

        public void WaitAny(TimeSpan? timeout = null)
        {
            if (journal != null)
            {
                foreach (var entry in journal.AfterLastAction())
                {
                    KeyValuePair<string[], Action<JournalEntry>> pair =
                        whenActions.FirstOrDefault(x => x.Key.Any(k => entry.Message.Contains(k)));
                    if (pair.Value != null)
                    {
                        journal.NotifyWait();
                        pair.Value(entry);
                        return;
                    }
                }

                journalSource.NewMessageReceived += JournalEntriesOnNewMessageReceived;
            }

            try
            {
                var totalWaitingMillieseconds = 0;
                while (!entryReceivedEvent.WaitOne(100))
                {
                    totalWaitingMillieseconds += 100;
                    if (timeout.HasValue && timeout.Value.TotalMilliseconds < totalWaitingMillieseconds)
                    {
                        timeoutAction?.Invoke();
                        return;
                    }

                    var token = tokenProvider();
                    token?.ThrowIfCancellationRequested();
                }

                var action = receivedAction;
                receivedAction = null;

                var entry = receivedJournalEntry;
                receivedJournalEntry = null;

                entryReceivedEvent.Reset();

                action(entry);
            }
            finally
            {
                if (journalSource != null)
                    journalSource.NewMessageReceived -= JournalEntriesOnNewMessageReceived;
            }
        }

        private void JournalEntriesOnNewMessageReceived(object sender, JournalEntry journalEntry)
        {
            ReceiveJournalEntry(journalEntry);
        }

        public JournalAwaiter WhenTimeout(Action timeoutAction)
        {
            this.timeoutAction = timeoutAction;

            return this;
        }
    }
}