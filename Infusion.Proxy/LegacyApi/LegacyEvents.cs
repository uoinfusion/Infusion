using System;

namespace Infusion.Proxy.LegacyApi
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

        internal void ResetEvents()
        {
            itemsObserver.ResetEvents();
        }
    }
}