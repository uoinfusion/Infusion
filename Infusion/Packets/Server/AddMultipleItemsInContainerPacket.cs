using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class AddMultipleItemsInContainerPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;

        public int ItemCount => ArrayPacketReader.ReadUShort(rawPacket.Payload, 3);

        public IEnumerable<Item> Items
        {
            get
            {
                var position = 5;

                for (var i = 0; i < ItemCount; i++)
                {
                    yield return new Item(
                        id: ArrayPacketReader.ReadId(rawPacket.Payload, position),
                        type: ArrayPacketReader.ReadModelId(rawPacket.Payload, position + 4),
                        amount: ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 7),
                        location: new Location3D(ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 9), ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 11), 0),
                        containerId: ArrayPacketReader.ReadId(rawPacket.Payload, position + 13),
                        color: (Color)ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 17),
                        layer: null
                    );

                    position += 19;
                }
            }
        }

        public override void Deserialize(Packet raw)
        {
            rawPacket = raw;
        }
    }
}