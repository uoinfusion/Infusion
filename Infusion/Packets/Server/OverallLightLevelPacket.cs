using System;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class OverallLightLevelPacket : MaterializedPacket
    {
        public byte Level { get; set; }

        public override Packet RawPacket
        {
            get
            {
                var payload = new byte[2];
                payload[0] = (byte) PacketDefinitions.OverallLightLevel.Id;
                payload[1] = Level;

                return new Packet(PacketDefinitions.OverallLightLevel.Id, payload);
            }
        }

        public override void Deserialize(Packet rawPacket)
        {
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            Level = reader.ReadByte();
        }
    }
}