using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;

namespace Infusion.LegacyApi.Tests.Packets
{
    public static class TargetCursorPackets
    {
        public static Packet TargetCursor = new Packet(PacketDefinitions.TargetCursor.Id, new byte[]
        {
            0x6C, 0x01, 0x00, 0x00, 0x00, 0x21, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00,
        });
    }
}
