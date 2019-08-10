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

        public SelectServerRequest()
        {
        }

        public Packet Serialize()
        {
            var payload = new byte[3];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.SelectServerRequest.Id);
            writer.WriteUShort(ChosenServerId);
            rawPacket = new Packet(PacketDefinitions.SelectServerRequest.Id, payload);

            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ChosenServerId = reader.ReadUShort();
        }
    }
}
