namespace UltimaRX.Packets.PacketDefinitions
{
    public abstract class PacketDefinition
    {
        protected PacketDefinition(int id, PacketLength length)
        {
            Id = id;
            Length = length;
        }

        public int Id { get; }

        private PacketLength Length { get; }

        public int GetSize(IPacketReader reader)
        {
            return Length.GetSize(reader);
        }
    }
}
