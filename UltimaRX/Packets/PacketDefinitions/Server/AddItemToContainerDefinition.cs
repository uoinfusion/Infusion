namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class AddItemToContainerDefinition : PacketDefinition
    {
        public AddItemToContainerDefinition() : base(Id, new StaticPacketLength(20))
        {
        }

        public new static int Id => 0x25;
    }
}