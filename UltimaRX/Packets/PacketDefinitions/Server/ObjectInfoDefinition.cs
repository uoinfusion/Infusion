namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class ObjectInfoDefinition : PacketDefinition
    {
        public ObjectInfoDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x1A;
    }
}