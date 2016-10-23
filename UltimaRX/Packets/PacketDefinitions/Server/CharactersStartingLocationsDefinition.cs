namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class CharactersStartingLocationsDefinition : PacketDefinition
    {
        public CharactersStartingLocationsDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xA9;
    }
}