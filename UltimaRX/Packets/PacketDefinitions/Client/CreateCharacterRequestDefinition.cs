using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class CreateCharacterRequestDefinition : PacketDefinition
    {
        public CreateCharacterRequestDefinition() : base(0, new StaticPacketLength(104))
        {
        }
    }
}
