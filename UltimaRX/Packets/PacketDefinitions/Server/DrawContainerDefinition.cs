namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class DrawContainerDefinition : PacketDefinition
    {
        public DrawContainerDefinition() : base(Id, new StaticPacketLength(7))
        {
        }

        public new static int Id => 0x24;
    }
}