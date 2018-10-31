using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    internal sealed class SpeechJournalSource : IEnumerable<JournalEntry>
    {
        private readonly object sourceLock = new object();
        internal const int MaxLength = 256;
        private ImmutableQueue<JournalEntry> journal = ImmutableQueue.Create<JournalEntry>();

        private readonly ThreadLocal<long> lastActionJournalEntryId;
        private readonly DiagnosticTrace trace;

        internal SpeechJournalSource(DiagnosticTrace trace = null)
        {
            lastActionJournalEntryId = new ThreadLocal<long>(false);
            this.trace = trace;
        }

        internal long LastActionJournalEntryId
        {
            get => lastActionJournalEntryId.Value;
            private set => lastActionJournalEntryId.Value = value;
        }

        public long CurrentJournalEntryId
        {
            get
            {
                lock (sourceLock)
                {
                    return currentJournalEntryId;
                }
            }
        }

        private long currentJournalEntryId;

        public IEnumerator<JournalEntry> GetEnumerator() => ((IEnumerable<JournalEntry>)journal).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)journal).GetEnumerator();

        public void NotifyAction()
        {
            trace?.Log($"NotifyAction LastActionJournalEntryId {LastActionJournalEntryId} -> CurrentJournalEntryId {CurrentJournalEntryId}");
            LastActionJournalEntryId = CurrentJournalEntryId;
        }

        public void AddMessage(string name, string message, ObjectId speakerId, ModelId type, Color color)
        {
            JournalEntry entry;

            lock (sourceLock)
            {
                if (currentJournalEntryId == long.MaxValue)
                    throw new InvalidOperationException("Maximum number of received journal entries exceeded, cannot continue receiving journal entries.");

                entry = new JournalEntry(CurrentJournalEntryId, name, message, speakerId, type, color);
                currentJournalEntryId++;

                journal = journal.Enqueue(entry);

                if (journal.Count() > MaxLength)
                    journal = journal.Dequeue();

            }

            OnNewMessageReceived(entry);
        }

        public event EventHandler<JournalEntry> NewMessageReceived;

        private void OnNewMessageReceived(JournalEntry newEntry)
        {
            NewMessageReceived?.Invoke(this, newEntry);
        }

        public IEnumerable<JournalEntry> AfterLastAction() => journal.Where(x => x.Id >= LastActionJournalEntryId);
    }
}
