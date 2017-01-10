using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class DrawObjectPacketTests
    {
        [TestMethod]
        public void Can_deserialize_packet_with_item_without_color()
        {
            var packetWithOneItemWithoutColor = FakePackets.Instantiate(new byte[]
            {
                0x78, // packet
                0x00, 0x1E, // length
                0x00, 0x00, 0x00, 0x01, // id
                0x01, 0x90, // type
                0x12, 0x9B, // xpos
                0x05, 0x53, // ypos
                0x0A, // zpos
                0x07, // direction/facing
                0x83, 0xEA, // color
                0x00, // status flag
                0x01, // notoriety
                0x40, 0x00, 0x00, 0x0C, // item1 id
                0x0F, 0x44, // item1 type
                0x02, // layer
                0x00, 0x00, 0x00, 0x00 // EOF
            });

            var materializedPacket = new DrawObjectPacket();
            materializedPacket.Deserialize(packetWithOneItemWithoutColor);
            var items = materializedPacket.Items.ToArray();

            items.Length.Should().Be(1);
            items.First().Id.Should().Be(0x4000000C);
            items.First().Type.Should().Be((ModelId)0x0F44);
            items.First().Amount.Should().Be(1);
            items.First().ContainerId.Should().Be(0x00000001u);
            items.First().Layer.Should().Be(Layer.TwoHandedWeapon);
        }

        [TestMethod]
        public void Can_deserialize_object_with_item_with_color()
        {
            var packetWithOneItemWithoutColor = FakePackets.Instantiate(new byte[]
            {
                0x78, // packet
                0x00, 0x20, // length
                0x00, 0x00, 0x00, 0x01, // id
                0x01, 0x90, // type
                0x12, 0x9B, // xpos
                0x05, 0x53, // ypos
                0x0A, // zpos
                0x07, // direction/facing
                0x83, 0xEA, // color
                0x00, // status flag
                0x01, // notoriety
                0x40, 0x00, 0x00, 0x02, // item id
                0xA0, 0x3B, // item type
                0x0B, // layer
                0x04, 0x4E, // color
                0x00, 0x00, 0x00, 0x00 // EOF
            });

            var materializedPacket = new DrawObjectPacket();
            materializedPacket.Deserialize(packetWithOneItemWithoutColor);
            var items = materializedPacket.Items.ToArray();

            items.Length.Should().Be(1);
            items.First().Id.Should().Be(0x40000002);
            items.First().Type.Should().Be((ModelId)(0xA03B - 0x8000));
            items.First().Amount.Should().Be(1);
            items.First().ContainerId.Should().Be(0x00000001u);
            items.First().Layer.Should().Be(Layer.Hair);
            items.First().Color.Should().Be((Color) 0x044E);
        }

        [TestMethod]
        public void Can_deserialize_object_without_items()
        {
            var drawObjectPacketWithoutItems = FakePackets.Instantiate(source: new byte[]
            {
                0x78, // packet
                0x00, 0x17, // length
                0x00, 0x00, 0x00, 0x01, // id
                0x01, 0x90, // type
                0x12, 0x9B, // xpos
                0x05, 0x53, // ypos
                0x0A, // zpos
                0x07, // direction/facing
                0x83, 0xEA, // color
                0x00, // status flag
                0x01, // notoriety
                0x00, 0x00, 0x00, 0x00 // EOF
            });

            var drawObjectPacket = new DrawObjectPacket();
            drawObjectPacket.Deserialize(drawObjectPacketWithoutItems);

            drawObjectPacket.Id.Should().Be(0x00000001);
            drawObjectPacket.Type.Should().Be((ModelId)0x0190);
            drawObjectPacket.Location.X.Should().Be(0x129B);
            drawObjectPacket.Location.Y.Should().Be(0x0553);
            drawObjectPacket.Location.Z.Should().Be(0x0A);
            drawObjectPacket.Direction.Should().Be(Direction.Northwest);
            drawObjectPacket.Notoriety.Should().Be(Notoriety.Innocent);
            drawObjectPacket.Color.Should().Be((Color) 0x83EA);
            drawObjectPacket.Items.Should().BeEmpty();
        }
    }
}