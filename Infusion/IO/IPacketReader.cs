namespace Infusion.IO
{
    public interface IPacketReader
    {
        byte ReadByte();
        ushort ReadUShort();
    }
}