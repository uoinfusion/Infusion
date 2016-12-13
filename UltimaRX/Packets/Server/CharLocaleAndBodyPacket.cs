using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class CharLocaleAndBodyPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public uint PlayerId { get; private set; }

        public ushort BodyType { get; set; }

        public Location3D Location { get; private set; }

        public Direction Direction { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Position = 1;

            PlayerId = reader.ReadUInt();
            uint unknown1 = reader.ReadUInt();
            BodyType = reader.ReadUShort();
            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            byte unknown2 = reader.ReadByte();
            byte zloc = reader.ReadByte();
            Location = new Location3D(xloc, yloc, zloc);
            Direction = (Direction) reader.ReadByte();
        }

        public override Packet RawPacket => rawPacket;
    }
}
