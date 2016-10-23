namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class NewHealthBarStatusUpdateDefinition : PacketDefinition
    {
        public NewHealthBarStatusUpdateDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x16;
    }
}