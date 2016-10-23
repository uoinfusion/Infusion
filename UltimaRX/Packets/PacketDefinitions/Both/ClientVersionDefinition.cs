namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class ClientVersionDefinition : PacketDefinition
    {
        public ClientVersionDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xBD;
    }
}