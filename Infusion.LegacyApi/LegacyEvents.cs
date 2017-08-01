using System;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class LegacyEvents
    {
        private readonly ItemsObservers itemsObserver;

        internal LegacyEvents(ItemsObservers itemsObserver)
        {
            this.itemsObserver = itemsObserver;
        }

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

    public struct ItemUseRequestedArgs
    {
        public ObjectId ItemId { get; }

        public ItemUseRequestedArgs(ObjectId itemId)
        {
            ItemId = itemId;
        }
    }
}