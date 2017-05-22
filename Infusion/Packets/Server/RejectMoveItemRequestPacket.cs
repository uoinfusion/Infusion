using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class RejectMoveItemRequestPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public byte Reason { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            Reason = reader.ReadByte();
        }

        public override Packet RawPacket => rawPacket;
    }
}
