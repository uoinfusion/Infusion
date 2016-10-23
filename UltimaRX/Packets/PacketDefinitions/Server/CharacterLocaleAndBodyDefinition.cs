namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class CharacterLocaleAndBodyDefinition : PacketDefinition
    {
        public CharacterLocaleAndBodyDefinition() : base(Id, new StaticPacketLength(37))
        {
        }

        public new static int Id => 0x1B;
    }
}