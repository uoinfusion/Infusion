namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class GameServerLoginRequestDefinition : PacketDefinition
    {
        public GameServerLoginRequestDefinition() : base(Id, new StaticPacketLength(65))
        {
        }

        public new static int Id => 0x91;
    }
}