namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class GetClientStatusDefinition : PacketDefinition
    {
        public GetClientStatusDefinition() : base(Id, new StaticPacketLength(10))
        {
        }

        public new static int Id => 0x34;
    }
}