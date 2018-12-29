using Infusion.Packets;

namespace Infusion.Clients
{
    internal class ClassicClientBehviorSince6017 : ClassicClientBehavior
    {
        public override void RegisterPackets()
        {
            base.RegisterPackets();

            PacketDefinitionRegistry.Register(PacketDefinitions.DropItem6017);
            PacketDefinitionRegistry.Register(PacketDefinitions.AddItemToContainer6017);
        }
    }
}
