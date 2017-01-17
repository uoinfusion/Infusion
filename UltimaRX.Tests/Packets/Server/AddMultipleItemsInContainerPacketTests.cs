using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class AddMultipleItemsInContainerPacketTests
    {
        private static readonly Packet AddMultipleItemsInContainerPacket = FakePackets.Instantiate(new byte[]
        {
            0x3C, // packet
            0x00, 0x2B, // size
            0x00, 0x02, // item count

            // first item
            0x40, 0x00, 0x00, 0x0B, // item id
            0x0E, 0xED, // item type
            0x00, // unknown
            0x01, 0xF4, // amount, stack
            0x00, 0x74, // xloc
            0x00, 0x61, // yloc
            0x40, 0x00, 0x00, 0x04, // container id
            0x00, 0x00, // color

            // second item
            0x40, 0x00, 0x00, 0x0A, // item id
            0x0F, 0xF1, // item type
            0x00, // unknown
            0x00, 0x01, // amount
            0x00, 0x43, // xloc
            0x00, 0x82, // yloc
            0x40, 0x00, 0x00, 0x04, // container id
            0x00, 0x00 // color
        });

        [TestMethod]
        public void Can_get_number_of_added_items()
        {
            var packet = new AddMultipleItemsInContainerPacket();
            packet.Deserialize(AddMultipleItemsInContainerPacket);

            packet.ItemCount.Should().Be(2);
        }

        [TestMethod]
        public void Can_enumerate_all_added_items()
        {
            var packet = new AddMultipleItemsInContainerPacket();
            packet.Deserialize(AddMultipleItemsInContainerPacket);

            packet.Items.Count().Should().Be(2);
        }

        [TestMethod]
        public void Can_deserizalize_first_item()
        {
            var packet = new AddMultipleItemsInContainerPacket();
            packet.Deserialize(AddMultipleItemsInContainerPacket);

            var firstItem = packet.Items.First();
            firstItem.Id.Should().Be(0x4000000b);
            firstItem.Type.Should().Be((ModelId)0x0eed);
            firstItem.Amount.Should().Be(0x01f4);
            firstItem.Color.Should().Be(new Color(0x0000));
            firstItem.ContainerId.Value.Should().Be(0x40000004);
            firstItem.Location.X.Should().Be(0x74);
            firstItem.Location.Y.Should().Be(0x61);
        }

        [TestMethod]
        public void Can_deserizalize_second_item()
        {
            var packet = new AddMultipleItemsInContainerPacket();
            packet.Deserialize(AddMultipleItemsInContainerPacket);

            var firstItem = packet.Items.Skip(1).First();
            firstItem.Id.Should().Be(0x4000000a);
            firstItem.Type.Should().Be((ModelId)0x0ff1);
            firstItem.Amount.Should().Be(1);
            firstItem.Color.Should().Be(new Color(0x0000));
            firstItem.ContainerId.Value.Should().Be(0x40000004);
            firstItem.Location.X.Should().Be(0x0043);
            firstItem.Location.Y.Should().Be(0x0082);
        }
    }
}