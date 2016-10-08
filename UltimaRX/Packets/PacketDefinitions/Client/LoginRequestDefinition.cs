namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class LoginRequestDefinition : PacketDefinition
    {
        public LoginRequestDefinition() : base(0x80, new StaticPacketLength(62))
        {
        }
    }
}