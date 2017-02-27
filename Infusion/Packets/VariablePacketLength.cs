using Infusion.IO;

namespace Infusion.Packets
{
    public class VariablePacketLength : PacketLength
    {
        public override int GetSize(IPacketReader reader)
        {
            return reader.ReadUShort();
        }
    }
}
