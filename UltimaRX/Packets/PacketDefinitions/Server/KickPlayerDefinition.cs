namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class KickPlayerDefinition : PacketDefinition
    {
        public KickPlayerDefinition() : base(Id, new StaticPacketLength(5))
        {
        }

        public new static int Id => 0x26;
    }
}