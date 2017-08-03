using Infusion.IO;

namespace Infusion.Packets
{
    internal abstract class PacketLength
    {
        public abstract int GetSize(IPacketReader reader);
    }
}