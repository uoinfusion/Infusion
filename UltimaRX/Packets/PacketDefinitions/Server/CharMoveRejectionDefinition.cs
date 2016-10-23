namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class CharMoveRejectionDefinition : PacketDefinition
    {
        public CharMoveRejectionDefinition() : base(Id, new StaticPacketLength(8))
        {
        }

        public new static int Id => 0x21;
    }
}