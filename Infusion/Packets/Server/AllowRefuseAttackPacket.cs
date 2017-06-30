using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class AllowRefuseAttackPacket : MaterializedPacket
    {
        public uint AttackTargetId { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            AttackTargetId = reader.ReadUInt();
        }

        public override Packet RawPacket => rawPacket;

        private Packet rawPacket;
    }
}
