using Infusion.Packets;

namespace Infusion.Proxy.LegacyApi
{
    public struct CurrentHealthUpdatedArgs
    {
        internal CurrentHealthUpdatedArgs(Item updatedItem, ushort oldHealth)
        {
            UpdatedItem = updatedItem;
            OldHealth = oldHealth;
        }

        public Item UpdatedItem { get; }
        public ushort OldHealth { get; }
    }
}