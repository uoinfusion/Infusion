using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Client
{
    internal sealed class SelectServerRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
        public ushort ChosenServerId { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ChosenServerId = reader.ReadUShort();
        }
    }
}
