namespace UltimaRX.Packets
{
    public abstract class MaterializedPacket
    {
        public abstract void Deserialize(Packet rawPacket);

        public abstract Packet RawPacket { get; }
    }
}