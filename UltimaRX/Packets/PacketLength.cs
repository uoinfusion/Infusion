using Infusion.IO;

namespace Infusion.Packets
{
    public abstract class PacketLength
    {
        public abstract int GetSize(IPacketReader reader);
    }
}