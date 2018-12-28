using System;
using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class DrawObjectPacket : MaterializedPacket
    {
        private Packet rawPacket;
        private ModelId type;


        public ObjectId Id { get; private set; }
        public ModelId Type { get => type;
            set
            {
                type = value;
                var writer = new ArrayPacketWriter(rawPacket.Payload);

                writer.Position = 1 + 2 + 4;
                writer.WriteModelId(value);
            }
        }
        public Location3D Location { get; private set; }

        public override Packet RawPacket => rawPacket;
        public Direction Direction { get; private set; }
        public MovementType MovementType { get; set; }
        public Color Color { get; private set; }
        public Notoriety Notoriety { get; private set; }
        public IEnumerable<Item> Items { get; private set; }
        public byte Flags { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            Id = reader.ReadObjectId();
            Type = reader.ReadModelId();
            Location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            (Direction, MovementType) = reader.ReadDirection();
            Color = (Color) reader.ReadUShort();
            Flags = reader.ReadByte();
            Notoriety = (Notoriety) reader.ReadByte();

            var items = new List<Item>();
            var itemId = reader.ReadUInt();
            while (itemId != 0x00000000)
            {
                var type = reader.ReadUShort();
                var layer = (Layer) reader.ReadByte();
                Color? color = null;

                DeserializeColor(reader, ref type, ref color);

                var item = new Item(new ObjectId(itemId), new ModelId(type), 1, new Location3D(0, 0, 0), containerId: Id,
                    layer: layer, color: color);

                items.Add(item);

                itemId = reader.ReadUInt();
            }

            Items = items.ToArray();
        }

        protected virtual void DeserializeColor(ArrayPacketReader reader, ref ushort type, ref Color? color)
        {
            if ((type & 0x8000) != 0)
            {
                type -= 0x8000;
                color = (Color)reader.ReadUShort();
            }
        }
    }
}