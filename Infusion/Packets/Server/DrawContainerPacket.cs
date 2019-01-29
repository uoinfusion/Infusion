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
        protected Packet rawPacket;
        private readonly int protocolVersion;
        public override Packet RawPacket => rawPacket;

        public ObjectId ContainerId { get; protected set; }

        public ModelId GumpModel { get; protected set; }

        public DrawContainerPacket(int protocolVersion = 0)
        {
            this.protocolVersion = protocolVersion;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ContainerId = reader.ReadObjectId();
            GumpModel = reader.ReadModelId();

            if (protocolVersion >= 7090)
                reader.ReadUShort();
        }
    }
}
