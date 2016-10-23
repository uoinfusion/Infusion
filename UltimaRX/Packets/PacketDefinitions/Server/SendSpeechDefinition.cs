namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class SendSpeechDefinition : PacketDefinition
    {
        public SendSpeechDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x1C;
    }
}