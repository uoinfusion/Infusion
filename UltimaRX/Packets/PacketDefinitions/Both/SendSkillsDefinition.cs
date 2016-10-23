using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Packets.PacketDefinitions.Both
{
    public class SendSkillsDefinition : PacketDefinition
    {
        public SendSkillsDefinition() : base(Id, new StandardVariablePacketLength())
        {
        }

        public new static int Id => 0x3A;
    }
}
