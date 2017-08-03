namespace Infusion.IO
{
    internal interface IPacketReader
    {
        byte ReadByte();
        ushort ReadUShort();
    }
}