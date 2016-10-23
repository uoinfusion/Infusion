namespace UltimaRX.Packets.PacketDefinitions
{
    public class DrawObjectDefinition : PacketDefinition
    {
        public DrawObjectDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x78;
    }
}