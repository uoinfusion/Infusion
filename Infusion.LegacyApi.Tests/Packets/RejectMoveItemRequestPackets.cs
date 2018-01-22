using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;

namespace Infusion.LegacyApi.Tests.Packets
{
    public static class RejectMoveItemRequestPackets
    {
        public static Packet CannotLiftTheItem { get; } =
            new Packet(PacketDefinitions.RejectMoveItemRequest.Id, new byte[] {0x27, 0x00});
    }
}
