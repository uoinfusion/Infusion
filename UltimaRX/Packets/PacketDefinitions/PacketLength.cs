namespace UltimaRX.Packets.PacketDefinitions
{
    public abstract class PacketLength
    {
        public abstract int GetSize(IPacketReader reader);
    }
}