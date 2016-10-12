using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class EnableLockedClientFeaturesDefinition : PacketDefinition
    {
        public EnableLockedClientFeaturesDefinition() : base(0xB9, new StaticPacketLength(3))
        {
        }
    }
}
