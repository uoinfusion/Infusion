using Infusion.IO;

namespace Infusion.Packets
{
    internal sealed class VariablePacketLength : PacketLength
    {
        public override int GetSize(IPacketReader reader)
        {
            return reader.ReadUShort();
        }
    }
}
