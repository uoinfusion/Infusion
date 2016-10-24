using UltimaRX.IO;

namespace UltimaRX.Packets
{
    public class VariablePacketLength : PacketLength
    {
        public override int GetSize(IPacketReader reader)
        {
            return reader.ReadUShort();
        }
    }
}
