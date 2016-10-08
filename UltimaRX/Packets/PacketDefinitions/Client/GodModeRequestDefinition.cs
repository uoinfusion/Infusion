namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class GodModeRequestDefinition : PacketDefinition
    {
        public GodModeRequestDefinition() : base(0x04, new StaticPacketLength(2))
        {
        }
    }
}