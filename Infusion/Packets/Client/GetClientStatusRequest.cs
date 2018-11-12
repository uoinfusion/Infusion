using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class GetClientStatusRequest : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId Id { get; private set; }
        public override Packet RawPacket => rawPacket;

        public GetClientStatusRequest()
        {
        }

        public GetClientStatusRequest(ObjectId id)
        {
            var payload = new byte[10];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GetClientStatus.Id);
            writer.WriteUInt(0xedededed);
            writer.WriteByte(0x04);
            writer.WriteId(id);

            this.Id = id;

            rawPacket = new Packet(PacketDefinitions.GetClientStatus.Id, payload);
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(6);
            Id = reader.ReadObjectId();
        }
    }
}
