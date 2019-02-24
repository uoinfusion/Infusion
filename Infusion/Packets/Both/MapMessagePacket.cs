using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Both
{
    internal sealed class MapMessagePacket : MaterializedPacket
    {
        private Packet rawPacket;

        public Location2D UpperLeft { get; set; }
        public Location2D LowerRight { get; set; }
        public ushort Width { get; set; }
        public ushort Height { get; set; }
        public ObjectId MapId { get; set; }
        public ModelId MapArt { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            MapId = reader.ReadObjectId();
            MapArt = reader.ReadModelId();
            UpperLeft = new Location2D(reader.ReadUShort(), reader.ReadUShort());
            LowerRight = new Location2D(reader.ReadUShort(), reader.ReadUShort());
            Width = reader.ReadUShort();
            Height = reader.ReadUShort();
        }
    }
}
