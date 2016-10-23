namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class GeneralInformationPacketDefinition : PacketDefinition
    {
        public GeneralInformationPacketDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xBF;
    }
}