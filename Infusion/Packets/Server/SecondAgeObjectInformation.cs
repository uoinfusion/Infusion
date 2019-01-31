using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets.Server
{
    internal sealed class SecondAgeObjectInformationPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId Id { get; private set; }

        public ModelId Type { get; private set; }

        public Direction Facing { get; private set; }

        public ushort Amount { get; private set; }

        public Location3D Location { get; private set; }

        public Color Color { get; private set; }

        public Layer Layer { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(4);

            Id = reader.ReadObjectId();
            Type = reader.ReadModelId();
            Facing = (Direction)reader.ReadByte();
            Amount = reader.ReadUShort();
            reader.ReadUShort(); // second amount?
            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            Layer = reader.ReadLayer();
            Color = reader.ReadColor();

            reader.ReadByte(); // flags
        }
    }
}
