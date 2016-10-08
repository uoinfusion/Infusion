using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class SelectServerRequestDefinition : PacketDefinition
    {
        public SelectServerRequestDefinition() : base(0xA0, new StaticPacketLength(3))
        {
        }
    }
}
