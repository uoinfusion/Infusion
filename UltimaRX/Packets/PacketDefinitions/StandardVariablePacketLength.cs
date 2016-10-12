namespace UltimaRX.Packets.PacketDefinitions
{
    public class StandardVariablePacketLength : PacketLength
    {
        public override int GetSize(IPacketReader reader)
        {
            return reader.ReadUShort();
        }
    }
}
