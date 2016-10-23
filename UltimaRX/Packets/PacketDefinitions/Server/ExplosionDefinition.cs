namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class ExplosionDefinition : PacketDefinition
    {
        public ExplosionDefinition() : base(Id, new StaticPacketLength(8))
        {
        }

        public new static int Id => 0x1F;
    }
}