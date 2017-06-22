using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Proxy.LegacyApi
{
    internal class JournalSource : IEnumerable<JournalEntry>
    {
        private ImmutableQueue<JournalEntry> journal = ImmutableQueue.Create<JournalEntry>();

        public IEnumerator<JournalEntry> GetEnumerator() => ((IEnumerable<JournalEntry>)journal).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)journal).GetEnumerator();

        internal void AddMessage(JournalEntry entry)
        {
            journal = journal.Enqueue(entry);

            OnNewMessageReceived(entry);
        }

        public event EventHandler<JournalEntry> NewMessageReceived;

        private void OnNewMessageReceived(JournalEntry newEntry)
        {
            NewMessageReceived?.Invoke(this, newEntry);
        }

    }
}
