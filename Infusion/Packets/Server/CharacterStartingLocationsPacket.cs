using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class CharactersStartingLocationsPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public CharacterItem[] Characters { get; set; }

        public CharactersStartingLocationsPacket()
        {
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            var charCount = reader.ReadByte();
            Characters = new CharacterItem[charCount];
            for (int i = 0; i < charCount; i++)
            {
                string name = reader.ReadString(30);
                string password = reader.ReadString(30);
                Characters[i] = new CharacterItem
                {
                    Name = name,
                };
            }
        }

        public override Packet RawPacket => this.rawPacket;
    }

    public sealed class CharacterItem
    {
        public string Name { get; set; }
    }
}
