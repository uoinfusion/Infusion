namespace Infusion.LegacyApi
{
    public struct ItemUseRequestedArgs
    {
        public ObjectId ItemId { get; }

        public ItemUseRequestedArgs(ObjectId itemId)
        {
            ItemId = itemId;
        }
    }
}