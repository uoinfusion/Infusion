using System;
using System.Collections.Generic;
using System.Linq;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal class DrawObjectPacket : MaterializedPacket
    {
        private Packet rawPacket;
        private ModelId type;
        private Location3D location;

        public ObjectId Id { get; set; }
        public ModelId Type { get => type;
            set
            {
                type = value;
                if (rawPacket.Payload != null)
                {
                    var writer = new ArrayPacketWriter(rawPacket.Payload);
                    writer.Position = 1 + 2 + 4;
                    writer.WriteModelId(value);
                }
            }
        }

        public Location3D Location
        {
            get => location;
            set
            {
                location = value;

                if (rawPacket.Payload != null)
                {
                    var writer = new ArrayPacketWriter(rawPacket.Payload);
                    writer.Position = 1 + 2 + 4 + 2;
                    writer.WriteUShort((ushort)value.X);
                    writer.WriteUShort((ushort)value.Y);
                    writer.WriteSByte((sbyte)value.Z);
                }
            }
        }

        public override Packet RawPacket => rawPacket;
        public Direction Direction { get; set; }
        public MovementType MovementType { get; set; }
        public Color Color { get; set; }
        public Notoriety Notoriety { get; set; }
        public IEnumerable<Item> Items { get; set; }
        public byte Flags { get; set; }

        private int GetLength(Item i) => i.Color.HasValue ? 9 : 7;


        public Packet Serialize()
        {
            var size = (ushort)(19 + Items.Select(i => GetLength(i)).Sum() + 4);
            var payload = new byte[size];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.DrawObject.Id);
            writer.WriteUShort(size);
            writer.WriteObjectId(Id);
            writer.WriteModelId(Type);
            writer.WriteUShort((ushort)Location.X);
            writer.WriteUShort((ushort)Location.Y);
            writer.WriteSByte((sbyte)Location.Z);
            writer.WriteDirection(Direction, MovementType);
            writer.WriteColor(Color);
            writer.WriteByte(Flags);
            writer.WriteByte((byte)Notoriety);

            foreach (var item in Items)
            {
                writer.WriteObjectId(item.Id);
                writer.WriteModelId(item.Color.HasValue ? (ModelId)(item.Type + 0x8000) : item.Type);
                writer.WriteByte((byte)item.Layer);
                if (item.Color.HasValue)
                    writer.WriteColor(item.Color.Value);
            }

            writer.WriteUInt(0);

            rawPacket = new Packet(payload);

            return rawPacket;
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(3);

            Id = reader.ReadObjectId();
            this.type = reader.ReadModelId();
            this.location = new Location3D(reader.ReadUShort(), reader.ReadUShort(), reader.ReadSByte());
            (Direction, MovementType) = reader.ReadDirection();
            Color = (Color) reader.ReadUShort();
            Flags = reader.ReadByte();
            Notoriety = (Notoriety) reader.ReadByte();

            var items = new List<Item>();
            var itemId = reader.ReadUInt();
            while (itemId != 0x00000000)
            {
                var itemType = reader.ReadUShort();
                var layer = (Layer) reader.ReadByte();
                Color? color = null;

                DeserializeTypeAndColor(reader, ref itemType, ref color);

                var item = new Item(new ObjectId(itemId), new ModelId(itemType), 1, new Location3D(0, 0, 0), containerId: Id,
                    layer: layer, color: color);

                items.Add(item);

                itemId = reader.ReadUInt();
            }

            Items = items.ToArray();
        }

        protected virtual void DeserializeTypeAndColor(ArrayPacketReader reader, ref ushort type, ref Color? color)
        {
            if ((type & 0x8000) != 0)
            {
                type -= 0x8000;
                color = (Color)reader.ReadUShort();
            }
        }
    }
}