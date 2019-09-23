using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Client
{
    internal sealed class ExtendedLoginSeed : MaterializedPacket
    {
        private Packet rawPacket;

        public byte[] Seed { get; set; }
        public Version ClientVersion { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);
            Seed = new byte[4];
            reader.Read(Seed, 0, 4);

            ClientVersion = new Version(reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt());
        }

        public Packet Serialize()
        {
            var payload = new byte[21];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte(0xEF);
            writer.Write(Seed, 0, 4);
            writer.WriteInt(ClientVersion.Major);
            writer.WriteInt(ClientVersion.Minor);
            writer.WriteInt(ClientVersion.Build);

            int revision = ClientVersion.Revision > -1 ? ClientVersion.Revision : 0;
            writer.WriteInt(revision);

            rawPacket = new Packet(0xFE, payload);
            return rawPacket;
        }
    }
}
