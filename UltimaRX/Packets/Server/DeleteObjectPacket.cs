using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class DeleteObjectPacket : MaterializedPacket
    {
        public uint Id { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            Id = ArrayPacketReader.ReadUInt(rawPacket.Payload, 1);
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
