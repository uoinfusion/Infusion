using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets.Client;

namespace UltimaRX.Packets.Server
{
    public class TargetCursorPacket
    {
        public TargetCursorPacket(CursorTarget cursorTarget, int cursorId, CursorType cursorType)
        {
            byte[] payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte)cursorTarget);
            writer.WriteInt(cursorId);
            writer.WriteByte((byte)cursorType);

            RawPacket = new Packet(PacketDefinitions.TargetCursor.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
