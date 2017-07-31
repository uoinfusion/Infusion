using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public struct CurrentHealthUpdatedArgs
    {
        internal CurrentHealthUpdatedArgs(Mobile updatedMobile, ushort oldHealth)
        {
            UpdatedMobile = updatedMobile;
            OldHealth = oldHealth;
        }

        public Mobile UpdatedMobile { get; }
        public ushort OldHealth { get; }
    }
}