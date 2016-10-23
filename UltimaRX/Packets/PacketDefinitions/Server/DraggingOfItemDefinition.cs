namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class DraggingOfItemDefinition : PacketDefinition
    {
        public DraggingOfItemDefinition() : base(Id, new StaticPacketLength(26))
        {
        }

        public new static int Id => 0x23;
    }
}