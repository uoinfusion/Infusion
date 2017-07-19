using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    internal sealed class JournalSource : IEnumerable<JournalEntry>
    {
        private readonly object sourceLock = new object();
        private const int MaxLength = 256;
        private ImmutableQueue<JournalEntry> journal = ImmutableQueue.Create<JournalEntry>();
        private long lastActionJournalEntryId;

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

        public void NotifyLastAction()
        {
            lastActionJournalEntryId = CurrentJournalEntryId;
        }

        public void AddMessage(string name, string message, uint speakerId, ModelId type)
        {
            JournalEntry entry;

            lock (sourceLock)
            {
                if (currentJournalEntryId == long.MaxValue)
                    throw new InvalidOperationException("Maximum number of received journal entries exceeded, cannot continue receiving journal entries.");

                entry = new JournalEntry(CurrentJournalEntryId, name, message, speakerId, type);
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

        public IEnumerable<JournalEntry> AfterLastAction() => journal.Where(x => x.Id >= lastActionJournalEntryId);
    }
}
