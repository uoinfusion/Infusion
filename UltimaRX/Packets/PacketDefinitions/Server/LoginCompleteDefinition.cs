namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class LoginCompleteDefinition : PacketDefinition
    {
        public LoginCompleteDefinition() : base(Id, new StaticPacketLength(1))
        {
        }

        public new static int Id => 0x55;
    }
}