namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class PingMessageDefinition : PacketDefinition
    {
        public PingMessageDefinition() : base(Id, new StaticPacketLength(2))
        {
        }

        public new static int Id => 0x73;
    }
}