namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class ConnectToGameServerDefinition : PacketDefinition
    {
        public ConnectToGameServerDefinition() : base(0x8C, new StaticPacketLength(11))
        {
        }
    }
}