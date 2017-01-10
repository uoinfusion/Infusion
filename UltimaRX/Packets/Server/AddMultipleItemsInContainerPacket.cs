using System.Collections.Generic;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public sealed class AddMultipleItemsInContainerPacket : MaterializedPacket
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
                        id: ArrayPacketReader.ReadUInt(rawPacket.Payload, position),
                        type: ArrayPacketReader.ReadModelId(rawPacket.Payload, position + 4),
                        amount: ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 7),
                        xLoc: ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 9),
                        yLoc: ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 11),
                        containerId: ArrayPacketReader.ReadUInt(rawPacket.Payload, position + 13),
                        color: ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 17)
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