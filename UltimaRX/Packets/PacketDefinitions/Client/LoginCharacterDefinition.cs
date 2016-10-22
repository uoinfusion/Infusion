namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class LoginCharacterDefinition : PacketDefinition
    {
        public LoginCharacterDefinition() : base(Id, new StaticPacketLength(73))
        {
        }

        public new static int Id => 0x5D;
    }
}