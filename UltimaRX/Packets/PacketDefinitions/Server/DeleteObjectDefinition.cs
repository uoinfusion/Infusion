namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class DeleteObjectDefinition : PacketDefinition
    {
        public DeleteObjectDefinition() : base(Id, new StaticPacketLength(5))
        {
        }

        public new static int Id => 0x1D;
    }
}