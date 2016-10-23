namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class DamageDefinition : PacketDefinition
    {
        public DamageDefinition() : base(Id, new StaticPacketLength(7))
        {
        }

        public new static int Id => 0x0B;
    }
}