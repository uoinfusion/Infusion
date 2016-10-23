namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class GraphicalEffectDefinition : PacketDefinition
    {
        public GraphicalEffectDefinition() : base(Id, new StaticPacketLength(28))
        {
        }

        public new static int Id => 0x70;
    }
}