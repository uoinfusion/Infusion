using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class TalkRequestDefinition : PacketDefinition
    {
        public TalkRequestDefinition() : base(0x03, new StandardVariablePacketLength())
        {
        }
    }
}
