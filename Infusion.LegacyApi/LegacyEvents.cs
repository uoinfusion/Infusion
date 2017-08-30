using System;
using System.Threading.Tasks;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class LegacyEvents
    {
        private readonly ItemsObservers itemsObserver;
        private readonly object speechReceivedLock = new object();

        internal LegacyEvents(ItemsObservers itemsObserver, JournalSource journalSource)
        {
            this.itemsObserver = itemsObserver;
            journalSource.NewMessageReceived += JournalSourceOnNewMessageReceived;
        }

        private void JournalSourceOnNewMessageReceived(object sender, JournalEntry journalEntry)
        {
            EventHelper.RaiseScriptEvent(SpeechReceived, () =>
            {
                lock (speechReceivedLock)
                    SpeechReceived?.Invoke(this, journalEntry);
            });
        }

        public event EventHandler<JournalEntry> SpeechReceived;

        public event EventHandler<CurrentHealthUpdatedArgs> HealthUpdated
        {
            add => itemsObserver.CurrentHealthUpdated += value;
            remove => itemsObserver.CurrentHealthUpdated -= value;
        }

        public event EventHandler<ItemEnteredViewArgs> ItemEnteredView
        {
            add => itemsObserver.ItemEnteredView += value;
            remove => itemsObserver.ItemEnteredView -= value;
        }

        public event EventHandler<ItemUseRequestedArgs> ItemUseRequested
        {
            add => itemsObserver.DoubleClickRequested += value;
            remove => itemsObserver.DoubleClickRequested -= value;
        }

        internal void ResetEvents()
        {
            itemsObserver.ResetEvents();
        }
    }
}