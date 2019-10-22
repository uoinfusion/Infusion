using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class EnableLockedClientFeatures60142 : EnableLockedClientFeaturesPacket
    {
        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            Flags = reader.ReadUInt();
        }

        public override Packet Serialize()
        {
            var payload = new byte[5];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.EnableLockedClientFeatures.Id);
            writer.WriteUInt((ushort)Flags);

            rawPacket = new Packet(PacketDefinitions.EnableLockedClientFeatures.Id, payload);

            return rawPacket;
        }
    }
}
