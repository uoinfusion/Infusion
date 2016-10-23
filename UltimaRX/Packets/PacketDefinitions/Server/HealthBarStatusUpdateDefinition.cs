namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class HealthBarStatusUpdateDefinition : PacketDefinition
    {
        public HealthBarStatusUpdateDefinition() : base(Id, new StaticPacketLength(12))
        {
        }

        public new static int Id => 0x17;
    }
}