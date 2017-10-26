namespace Infusion.LegacyApi.Events
{
    public struct ItemEnteredViewEvent
    {
        internal ItemEnteredViewEvent(Item newItem)
        {
            NewItem = newItem;
        }

        public Item NewItem { get; set; }
    }
}