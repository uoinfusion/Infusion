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

        public ModelId BodyType { get; set; }

        public Location3D Location { get; private set; }

        public Movement Movement { get; set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            uint unknown1 = reader.ReadUInt();
            BodyType = reader.ReadModelId();
            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            byte unknown2 = reader.ReadByte();
            byte zloc = reader.ReadByte();
            Location = new Location3D(xloc, yloc, zloc);
            Movement = (Movement)reader.ReadByte();
        }

        public override Packet RawPacket => rawPacket;
    }
}
