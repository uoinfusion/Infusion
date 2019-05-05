using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Server
{
    internal sealed class RunUOProtocolExtensionPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
        public byte Type { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);
            Type = reader.ReadByte();
        }
    }
}
