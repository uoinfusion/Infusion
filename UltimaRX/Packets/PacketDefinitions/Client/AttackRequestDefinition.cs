namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class AttackRequestDefinition : PacketDefinition
    {
        public AttackRequestDefinition() : base(0x05, new StaticPacketLength(5))
        {
        }
    }
}