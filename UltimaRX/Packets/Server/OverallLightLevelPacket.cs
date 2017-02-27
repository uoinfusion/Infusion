using System;

namespace Infusion.Packets.Server
{
    public class OverallLightLevelPacket : MaterializedPacket
    {
        public override void Deserialize(Packet rawPacket)
        {
            throw new NotImplementedException();
        }

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
    }
}
