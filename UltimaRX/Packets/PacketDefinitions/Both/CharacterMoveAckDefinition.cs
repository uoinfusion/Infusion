namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class CharacterMoveAckDefinition : PacketDefinition
    {
        public CharacterMoveAckDefinition() : base(Id, new StaticPacketLength(3))
        {
        }

        public new static int Id => 0x22;
    }
}