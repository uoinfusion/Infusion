using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class AddMultipleItemsInContainerPacket : MaterializedPacket
    {
        private Packet rawPacket;
        private readonly int protocolVersion;

        public override Packet RawPacket => rawPacket;

        public int ItemCount => ArrayPacketReader.ReadUShort(rawPacket.Payload, 3);

        public AddMultipleItemsInContainerPacket(int protocolVersion = 0)
        {
            this.protocolVersion = protocolVersion;
        }

        public IEnumerable<Item> Items
        {
            get
            {
                var position = 5;

                for (var i = 0; i < ItemCount; i++)
                {
                    var id = ArrayPacketReader.ReadId(rawPacket.Payload, position);
                    var type = ArrayPacketReader.ReadModelId(rawPacket.Payload, position + 4);
                    var amount = ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 7);
                    var location = new Location3D(ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 9), ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 11), 0);

                    ObjectId containerId;
                    Color color;

                    if (protocolVersion < 6017)
                    {
                        containerId = ArrayPacketReader.ReadId(rawPacket.Payload, position + 13);
                        color = (Color)ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 17);
                        position += 19;
                    }
                    else
                    {
                        containerId = ArrayPacketReader.ReadId(rawPacket.Payload, position + 14);
                        color = (Color)ArrayPacketReader.ReadUShort(rawPacket.Payload, position + 18);
                        position += 20;
                    }

                    yield return new Item(
                        id: id,
                        type: type,
                        amount: amount,
                        location: location,
                        containerId: containerId,
                        color: color,
                        layer: null
                    );
                }
            }
        }

        public override void Deserialize(Packet raw)
        {
            rawPacket = raw;
        }
    }
}