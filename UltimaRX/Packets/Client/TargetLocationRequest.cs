using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Client
{
    public class TargetLocationRequest
    {
        public TargetLocationRequest(uint cursorId, Location3D location, ushort tileType, CursorType cursorType)
        {
            byte[] payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte)CursorTarget.Location);
            writer.WriteUInt(cursorId);
            writer.WriteByte((byte)cursorType); 
            writer.WriteInt(0); // clicked on item = 0 (using location click)
            writer.WriteUShort(location.X);
            writer.WriteUShort(location.Y);
            writer.WriteByte(0); // unknown
            writer.WriteByte(location.Z);
            writer.WriteUShort(tileType);

            RawPacket = new Packet(PacketDefinitions.TargetCursor.Id, payload);
        }

        public Packet RawPacket { get; }
    }

    public enum CursorTarget : byte
    {
        Object = 0,
        Location = 1
    }

    public enum CursorType : byte
    {
        Neutral = 0,
        Harmful = 1,
        Helpful = 2,
        Cancel = 3
    }
}
