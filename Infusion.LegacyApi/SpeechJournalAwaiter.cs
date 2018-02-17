using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Infusion.LegacyApi
{
    public sealed class SpeechJournalAwaiter
    {
        private readonly EventWaitHandle entryReceivedEvent = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly SpeechJournal journal;
        private readonly Func<TimeSpan?> defaultTimeout;
        private readonly Cancellation cancellation;
        private readonly JournalSource journalSource;

        private readonly Dictionary<string[], Action<JournalEntry>> whenActions =
            new Dictionary<string[], Action<JournalEntry>>();

        private Action<JournalEntry> receivedAction;
        private JournalEntry receivedJournalEntry;

        private Action timeoutAction;

        internal SpeechJournalAwaiter(Cancellation cancellation, JournalSource journalSource = null,
            SpeechJournal journal = null, Func<TimeSpan?> defaultTimeout = null)
        {
            this.cancellation = cancellation;
            this.journalSource = journalSource;
            this.journal = journal;
            this.defaultTimeout = defaultTimeout;
        }

        internal void ReceiveJournalEntry(JournalEntry entry)
        {
            var keyValuePair =
                whenActions.FirstOrDefault(pair => pair.Key.Any(awaitedWord =>
                    entry.Text.IndexOf(awaitedWord, StringComparison.OrdinalIgnoreCase) >= 0));
            if (keyValuePair.Key != null && keyValuePair.Value != null)
            {
                receivedAction = keyValuePair.Value;
                receivedJournalEntry = entry;

                journal?.NotifyWait();
                entryReceivedEvent.Set();
            }
        }

        public SpeechJournalAwaiter When(string awaitedWord1, Action whenAction)
        {
            whenActions[new[] {awaitedWord1}] = entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string[] awaitedWords, Action whenAction)
        {
            whenActions[awaitedWords] = entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2}] = entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3}] = entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4}] = entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5}] =
                entry => whenAction();

            return this;
        }

        public SpeechJournalAwaiter When(string[] awaitedWords, Action<JournalEntry> whenAction)
        {
            whenActions[awaitedWords] = whenAction;

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1}] = whenAction;

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2}] = whenAction;

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3,
            Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3}] = whenAction;

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4}] = whenAction;

            return this;
        }

        public SpeechJournalAwaiter When(string awaitedWord1, string awaitedWord2, string awaitedWord3, string awaitedWord4,
            string awaitedWord5, Action<JournalEntry> whenAction)
        {
            whenActions[new[] {awaitedWord1, awaitedWord2, awaitedWord3, awaitedWord4, awaitedWord5}] = whenAction;

            return this;
        }

        public void WaitAny(TimeSpan? timeout = null)
        {
            timeout = timeout ?? defaultTimeout?.Invoke();

            JournalEntry beforeWaitAny = null;
            long? lastWaitEntryId = null;

            if (journalSource != null)
            {
                journalSource.NewMessageReceived += JournalEntriesOnNewMessageReceived;
                beforeWaitAny = journalSource.LastOrDefault();
            }

            if (journal != null)
            {
                lastWaitEntryId = journal.LastWaitEntryId;
                foreach (var entry in journal.AfterLastAction())
                {
                    var pair =
                        whenActions.FirstOrDefault(x =>
                            x.Key.Any(k => entry.Message.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0));
                    if (pair.Value != null)
                    {
                        journal.NotifyWait();
                        pair.Value(entry);
                        return;
                    }
                }
            }

            try
            {
                var totalWaitingMillieseconds = 0;
                while (!entryReceivedEvent.WaitOne(100))
                {
                    totalWaitingMillieseconds += 100;
                    if (timeout.HasValue && timeout.Value.TotalMilliseconds < totalWaitingMillieseconds)
                    {
                        if (timeoutAction != null)
                            timeoutAction.Invoke();
                        else
                            throw new TimeoutException(BuildTimeoutDiagnosticInfo(beforeWaitAny, timeout,
                                lastWaitEntryId));

                        return;
                    }

                    cancellation?.Check();
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

        private string BuildTimeoutDiagnosticInfo(JournalEntry beforeWaitAny, TimeSpan? timeout, long? lastWaitEntryId)
        {
            var info = new StringBuilder();

            info.AppendLine(timeout.HasValue ? $"Journal WaitAny timeout after {timeout.Value}" : "Journal WaitAny timeout after unspecified TimeSpan");
            if (lastWaitEntryId.HasValue)
                info.AppendLine($"Jurnal's LastWaitEntryId is {lastWaitEntryId.Value}");

            if (beforeWaitAny != null)
            {
                info.AppendLine("Last journal entry before wait:");
                info.AppendLine(beforeWaitAny.ToString());

                var entriesAfterWait = journalSource?
                    .Where(x => x.Id > beforeWaitAny.Id).ToArray();
                if (entriesAfterWait != null && entriesAfterWait.Any())
                {
                    info.AppendLine("Journal entries after wait:");
                    foreach (var entry in entriesAfterWait)
                    {
                        info.AppendLine(entry.ToString());
                    }
                }
                else
                {
                    info.AppendLine("No journal entries after wait.");
                }
            }
            else
            {
                info.AppendLine("No entry before wait");
            }

            return info.ToString();
        }

        private void JournalEntriesOnNewMessageReceived(object sender, JournalEntry journalEntry)
        {
            ReceiveJournalEntry(journalEntry);
        }

        public SpeechJournalAwaiter WhenTimeout(Action timeoutAction)
        {
            this.timeoutAction = timeoutAction;

            return this;
        }
    }
}