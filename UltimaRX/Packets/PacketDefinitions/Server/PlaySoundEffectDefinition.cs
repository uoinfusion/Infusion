namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class PlaySoundEffectDefinition : PacketDefinition
    {
        public PlaySoundEffectDefinition() : base(Id, new StaticPacketLength(12))
        {
        }

        public new static int Id => 0x54;
    }
}