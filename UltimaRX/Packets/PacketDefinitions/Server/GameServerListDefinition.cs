namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class GameServerListDefinition : PacketDefinition
    {
        public GameServerListDefinition() : base(0xA8, new StandardVariablePacketLength())
        {
        }
    }
}