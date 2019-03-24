using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class DrawObjectPacket7033 : DrawObjectPacket
    {
        protected override void DeserializeTypeAndColor(ArrayPacketReader reader, ref ushort type, ref Color? color)
        {
            if ((type & 0x8000) != 0)
            {
                type -= 0x8000;
            }
            color = (Color)reader.ReadUShort();
        }
    }
}