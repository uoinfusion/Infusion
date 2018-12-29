using Infusion.Packets;

namespace Infusion.Clients
{
    internal class ClassicClientBehaviorSince7090 : ClassicClientBehaviorSince7000
    {
        public override void RegisterPackets()
        {
            base.RegisterPackets();

            PacketDefinitionRegistry.Register(PacketDefinitions.DrawContainer7090);
            PacketDefinitionRegistry.Register(PacketDefinitions.SecondAgeObjectInformation7090);
        }
    }
}
