using System;
using System.Threading.Tasks;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class LegacyEvents
    {
        private readonly ItemsObservers itemsObserver;
        private readonly object speechReceivedLock = new object();
        private readonly SoundObserver soundObserver;

        internal LegacyEvents(ItemsObservers itemsObserver, JournalSource journalSource, SoundObserver soundObserver)
        {
            this.soundObserver = soundObserver;
            this.itemsObserver = itemsObserver;
            journalSource.NewMessageReceived += JournalSourceOnNewMessageReceived;
        }

        private void JournalSourceOnNewMessageReceived(object sender, JournalEntry journalEntry)
        {
            SpeechReceived.RaiseScriptEvent(this, journalEntry);
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

        public event EventHandler<MobileEnteredViewArgs> MobileEnteredView
        {
            add => itemsObserver.MobileEnteredView += value;
            remove => itemsObserver.MobileEnteredView -= value;
        }

        public event EventHandler<ItemUseRequestedArgs> ItemUseRequested
        {
            add => itemsObserver.DoubleClickRequested += value;
            remove => itemsObserver.DoubleClickRequested -= value;
        }

        public event EventHandler<SoundEffectPlayedArgs> SoundEffectPlayed
        {
            add => soundObserver.SoundEffectPlayed += value;
            remove => soundObserver.SoundEffectPlayed -= value;
        }

        internal void ResetEvents()
        {
            itemsObserver.ResetEvents();
        }
    }
}