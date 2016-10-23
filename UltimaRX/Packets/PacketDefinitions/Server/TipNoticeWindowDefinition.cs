namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class TipNoticeWindowDefinition : PacketDefinition
    {
        public TipNoticeWindowDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xA6;
    }
}