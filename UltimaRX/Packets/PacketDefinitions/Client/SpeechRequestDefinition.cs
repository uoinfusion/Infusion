using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Client
{
    public class SpeechRequestDefinition : PacketDefinition
    {
        public SpeechRequestDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0xAD;
    }
}
