using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Client
{
    internal sealed class RazorNegotiateResponse
    {
        public byte ResponseCode { get; set; }

        public Packet RawPacket { get; }

        public RazorNegotiateResponse(byte responseCode)
        {
            ResponseCode = responseCode;

            var payload = new byte[4];
            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte(0xF0);
            writer.WriteUShort(4);
            writer.WriteByte(responseCode);

            RawPacket = new Packet(0xF0, payload);
        }
    }
}
