namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class ClientViewRangeDefinition : PacketDefinition
    {
        public ClientViewRangeDefinition() : base(Id, new StaticPacketLength(2))
        {
        }

        public new static int Id => 0xC8;
    }
}