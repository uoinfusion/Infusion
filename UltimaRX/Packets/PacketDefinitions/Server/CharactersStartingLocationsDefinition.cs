using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class CharactersStartingLocationsDefinition : PacketDefinition
    {
        public CharactersStartingLocationsDefinition() : base(0xA9, new StandardVariablePacketLength())
        {
        }
    }
}
