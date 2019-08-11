using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Both
{
    public sealed class PingPacket
    {
        public byte SequenceNumber { get; set; }

        public Packet Serialize()
        {
            var payload = new byte[2];

            payload[0] = (byte)PacketDefinitions.PingMessage.Id;
            payload[1] = SequenceNumber;

            return new Packet(PacketDefinitions.PingMessage.Id, payload);
        }
    }
}
