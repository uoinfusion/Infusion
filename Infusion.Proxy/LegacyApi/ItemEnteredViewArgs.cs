using Infusion.Packets;

namespace Infusion.Proxy.LegacyApi
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