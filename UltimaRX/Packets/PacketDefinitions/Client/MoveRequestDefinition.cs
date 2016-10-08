using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class MoveRequestDefinition : PacketDefinition
    {
        public MoveRequestDefinition() : base(0x02, new StaticPacketLength(7))
        {
        }
    }
}
