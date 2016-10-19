namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class ClientSpyDefinition : PacketDefinition
    {
        public ClientSpyDefinition() : base(Id, new StaticPacketLength(149))
        {
        }

        public new static int Id => 0xA4;
    }
}