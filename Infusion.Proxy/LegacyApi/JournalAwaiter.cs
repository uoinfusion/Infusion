using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Infusion.Proxy.LegacyApi
{
    public class JournalAwaiter
    {
        private readonly EventWaitHandle entryReceivedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly JournalEntries journalEntries;
        private readonly Func<CancellationToken?> tokenProvider;

        private readonly Dictionary<string[], Action<JournalEntry>> whenActions =
            new Dictionary<string[], Action<JournalEntry>>();

        private Action<JournalEntry> receivedAction;
        private JournalEntry receivedJournalEntry;

        private Action timeoutAction;

        public JournalAwaiter(Func<CancellationToken?> tokenProvider, JournalEntries journalEntries = null)
        {
            this.tokenProvider = tokenProvider;
            this.journalEntries = journalEntries;
        }

        public void ReceiveJournalEntry(JournalEntry entry)
        {
            var keyValuePair =
                whenActions.FirstOrDefault(pair => pair.Key.Any(awaitedWord => entry.Text.Contains(awaitedWord)));
            if (keyValuePair.Key != null && keyValuePair.Value != null)
            {
                receivedAction = keyValuePair.Value;
                receivedJournalEntry = entry;

                entryReceivedEvent.Set();
            }
        }

        public JournalAwaiter When(string awaitedWord, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord}] = whenAction;

            return this;
        }

        public JournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction)
        {
            whenActions[awaitedWords] = whenAction;

            return this;
        }

        public void Wait() => Wait(null);

        public void Wait(TimeSpan? timeout)
        {
            if (journalEntries != null)
                journalEntries.NewMessageReceived += JournalEntriesOnNewMessageReceived;

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
                if (journalEntries != null)
                    journalEntries.NewMessageReceived -= JournalEntriesOnNewMessageReceived;
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