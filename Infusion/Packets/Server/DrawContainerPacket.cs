using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class DrawContainerPacket : MaterializedPacket
    {
        public ObjectId ContainerId { get; protected set; }

        public ModelId GumpModel { get; protected set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ContainerId = reader.ReadObjectId();
            GumpModel = reader.ReadModelId();
        }

        public override Packet RawPacket => rawPacket;

        protected Packet rawPacket;

    }
}
