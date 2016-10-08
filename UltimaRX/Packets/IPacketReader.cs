namespace UltimaRX.Packets
{
    public interface IPacketReader
    {
        byte ReadByte();
        ushort ReadUShort();
    }
}