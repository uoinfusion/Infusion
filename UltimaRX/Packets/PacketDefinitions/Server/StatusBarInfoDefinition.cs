namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class StatusBarInfoDefinition : PacketDefinition
    {
        public StatusBarInfoDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x11;
    }
}