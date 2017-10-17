namespace Infusion.LegacyApi.Events
{
    public sealed class ItemUseRequestedEvent : IEvent
    {
        public ObjectId ItemId { get; }

        internal ItemUseRequestedEvent(ObjectId itemId)
        {
            ItemId = itemId;
        }
    }
}