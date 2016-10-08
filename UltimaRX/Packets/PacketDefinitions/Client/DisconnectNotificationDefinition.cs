using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class DisconnectNotificationDefinition : PacketDefinition
    {
        public DisconnectNotificationDefinition() : base(0x01, new StaticPacketLength(5))
        {
        }
    }
}
