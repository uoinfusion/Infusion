namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class RequestWarModeDefinition : PacketDefinition
    {
        public RequestWarModeDefinition() : base(Id, new StaticPacketLength(5))
        {
        }

        public new static int Id => 0x72;
    }
}