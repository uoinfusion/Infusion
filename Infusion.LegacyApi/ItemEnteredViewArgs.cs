using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public struct ItemEnteredViewArgs
    {
        internal ItemEnteredViewArgs(Item newItem)
        {
            NewItem = newItem;
        }

        public Item NewItem { get; set; }
    }
}