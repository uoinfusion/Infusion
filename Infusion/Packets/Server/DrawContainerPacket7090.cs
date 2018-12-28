using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class DrawContainerPacket7090 : DrawContainerPacket
    {
        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ContainerId = reader.ReadObjectId();
            GumpModel = reader.ReadModelId();

            reader.ReadUShort(); // ignored
        }
    }
}
