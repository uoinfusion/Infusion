namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class PauseClientDefinition : PacketDefinition
    {
        public PauseClientDefinition() : base(Id, new StaticPacketLength(2))
        {
        }

        public new static int Id => 0x33;
    }
}