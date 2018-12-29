using Infusion.Packets;

namespace Infusion.Clients
{
    internal class ClassicClientBehaviorSince7000 : ClassicClientBehaviorSince60142
    {
        public override void RegisterPackets()
        {
            base.RegisterPackets();

            PacketDefinitionRegistry.Register(PacketDefinitions.DrawObject7000);
        }
    }
}
