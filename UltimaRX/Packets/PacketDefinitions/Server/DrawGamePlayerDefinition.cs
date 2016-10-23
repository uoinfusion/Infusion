namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class DrawGamePlayerDefinition : PacketDefinition
    {
        public DrawGamePlayerDefinition() : base(Id, new StaticPacketLength(19))
        {
        }

        public new static int Id => 0x20;
    }
}