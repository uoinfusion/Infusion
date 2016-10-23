namespace UltimaRX.Packets
{
    public abstract class MaterializedPacket
    {
        protected MaterializedPacket(Packet rawPacket)
        {
            RawPacket = rawPacket;
        }

        public Packet RawPacket { get; }

        public int Id => RawPacket.Id;
    }
}