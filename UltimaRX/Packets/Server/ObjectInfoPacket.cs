using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class ObjectInfoPacket : MaterializedPacket
    {
        public ushort Type { get; private set; }

        public int Id { get; private set; }

        public ushort Amount { get; private set; }

        public Location3D Location { get; private set; }

        public ObjectFlag Flags { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Position = 3;
            uint rawId = reader.ReadUInt();
            Type = reader.ReadUShort();

            if ((rawId & 0x80000000) != 0)
            {
                Id = (int)(rawId - 0x80000000);
                Amount = reader.ReadUShort();
            }
            else
            {
                Amount = 1;
                Id = (int)rawId;
            }

            if ((Type & 0x8000) != 0)
            {
                throw new PacketParsingException(rawPacket, "Not implementated: Type & 0x8000");
            }

            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();

            if ((xloc & 0x8000) != 0)
            {
                throw new PacketParsingException(rawPacket, "Not implementated: xloc & 0x8000");
            }

            byte zloc = reader.ReadByte();

            if ((yloc & 0x8000) != 0)
            {
                throw new PacketParsingException(rawPacket, "Not implementated: yloc & 0x8000");
            }

            if ((yloc & 0x4000) != 0)
            {
                yloc -= 0x4000;
                this.Flags = (ObjectFlag)reader.ReadByte();
            }

            Location = new Location3D(xloc, yloc, zloc);
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }
}
