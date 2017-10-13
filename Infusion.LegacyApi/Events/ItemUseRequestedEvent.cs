namespace Infusion.LegacyApi.Events
{
    public struct ItemUseRequestedEvent
    {
        public ObjectId ItemId { get; }

        public ItemUseRequestedEvent(ObjectId itemId)
        {
            ItemId = itemId;
        }
    }
}