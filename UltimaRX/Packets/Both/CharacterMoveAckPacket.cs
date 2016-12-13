using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.Both
{
    public class CharacterMoveAckPacket : MaterializedPacket
    {
        public byte MovementSequenceKey { get; set; }
        public byte NotorietyFlag { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            MovementSequenceKey = rawPacket.Payload[1];
            NotorietyFlag = rawPacket.Payload[2];
        }

        public override Packet RawPacket
        {
            get
            {
                var payload = new byte[3];

                payload[0] = (byte)PacketDefinitions.CharacterMoveAck.Id;
                payload[1] = MovementSequenceKey;
                payload[2] = NotorietyFlag;

                return new Packet(PacketDefinitions.CharacterMoveAck.Id, payload);
            }
        }
    }
}
