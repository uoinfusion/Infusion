namespace Infusion.LegacyApi.Events
{
    public sealed class ItemEnteredViewEvent : IEvent
    {
        internal ItemEnteredViewEvent(Item newItem)
        {
            NewItem = newItem;
        }

        public Item NewItem { get; set; }
    }
}