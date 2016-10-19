namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class SelectServerRequestDefinition : PacketDefinition
    {
        public SelectServerRequestDefinition() : base(Id, new StaticPacketLength(3))
        {
        }

        public new static int Id => 0xA0;
    }
}