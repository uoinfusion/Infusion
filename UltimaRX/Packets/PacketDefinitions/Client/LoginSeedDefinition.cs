namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class LoginSeedDefinition : PacketDefinition
    {
        public LoginSeedDefinition() : base(Id, new StaticPacketLength(4))
        {
        }

        public new static int Id => -1;
    }
}