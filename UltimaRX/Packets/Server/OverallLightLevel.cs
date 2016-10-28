using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.Server
{
    public class OverallLightLevel : MaterializedPacket
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
