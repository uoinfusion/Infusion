using UltimaRX.IO;

namespace UltimaRX.Packets
{
    public class StandardVariablePacketLength : PacketLength
    {
        public override int GetSize(IPacketReader reader)
        {
            return reader.ReadUShort();
        }
    }
}
