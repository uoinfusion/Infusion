using System;

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

        internal void ResetEvents()
        {
            itemsObserver.ResetEvents();
        }
    }
}