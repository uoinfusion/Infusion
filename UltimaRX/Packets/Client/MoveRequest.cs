using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.Client
{
    public class MoveRequest : MaterializedPacket
    {
        public Direction Direction { get; set; }
        public byte SequenceKey { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            Direction = (Direction)rawPacket.Payload[1];
            SequenceKey = rawPacket.Payload[2];
        }

        public override Packet RawPacket
        {
            get
            {
                var payload = new byte[7];

                payload[0] = (byte)PacketDefinitions.MoveRequest.Id;
                payload[1] = (byte) Direction;
                payload[2] = SequenceKey;
                payload[3] = 0;
                payload[4] = 0;
                payload[5] = 0;
                payload[6] = 0;

                return new Packet(PacketDefinitions.MoveRequest.Id, payload);
            }
        }
    }
}
