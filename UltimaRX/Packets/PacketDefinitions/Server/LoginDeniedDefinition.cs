namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class LoginDeniedDefinition : PacketDefinition
    {
        public LoginDeniedDefinition() : base(Id, new StaticPacketLength(2))
        {
        }

        public new static int Id => 0x82;
    }
}