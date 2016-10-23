using UltimaRX.IO;

namespace UltimaRX.Packets
{
    public abstract class PacketLength
    {
        public abstract int GetSize(IPacketReader reader);
    }
}