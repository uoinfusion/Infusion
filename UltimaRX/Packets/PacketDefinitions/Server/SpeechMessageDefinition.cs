using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Server
{
    public class SpeechMessageDefinition : PacketDefinition
    {
        public SpeechMessageDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xAE;
    }
}
