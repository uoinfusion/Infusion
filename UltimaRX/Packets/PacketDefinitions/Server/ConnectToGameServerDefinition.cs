namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class ConnectToGameServerDefinition : PacketDefinition
    {
        public ConnectToGameServerDefinition() : base(Id, new StaticPacketLength(11))
        {
        }

        public new static int Id => 0x8C;

        protected override MaterializedPacket MaterializeImpl(Packet rawPacket)
        {
            return new ConnectToGameServer(rawPacket);
        }
    }
}