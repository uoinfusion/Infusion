using Infusion.Packets;

namespace Infusion.Clients
{
    internal class ClassicClientBehaviorSince60142 : ClassicClientBehviorSince6017
    {
        public override void RegisterPackets()
        {
            base.RegisterPackets();

            PacketDefinitionRegistry.Register(PacketDefinitions.EnableLockedClientFeaturesSince6_0_14_2);
        }
    }
}
