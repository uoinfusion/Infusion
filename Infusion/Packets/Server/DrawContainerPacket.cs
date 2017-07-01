using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class DrawContainerPacket : MaterializedPacket
    {
        public uint ContainerId { get; private set; }

        public ModelId GumpModel { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ContainerId = reader.ReadUInt();
            GumpModel = reader.ReadModelId();
        }

        public override Packet RawPacket => rawPacket;

        private Packet rawPacket;

    }
}
