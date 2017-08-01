using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Both
{
    [TestClass]
    public class TargetCursorPacketTests
    {
        [TestMethod]
        public void Can_deserialize_when_item_targeted()
        {
            var rawPacket = FakePackets.Instantiate(source: new byte[]
            {
                0x6C, // packet
                0x00, // CursorTarget
                0xDE, 0xAD, 0xBE, 0xEF, // cursor id
                0x00, // CursorType
                0x40, 0x00, 0x00, 0x0B, // clicked on item id
                0x00, 0x80, // xloc
                0x00, 0x6D, // yloc
                0x00, // unknown
                0x00, // zloc
                0x0E, 0xED // type
            });

            var materializedPacket = new TargetCursorPacket();
            materializedPacket.Deserialize(rawPacket);

            materializedPacket.CursorTarget.Should().Be(CursorTarget.Object);
            materializedPacket.CursorId.Should().Be(new CursorId(0xDEADBEEF));
            materializedPacket.CursorType.Should().Be(CursorType.Neutral);
            materializedPacket.ClickedOnId.Should().Be(new ObjectId(0x4000000B));
            materializedPacket.Location.X.Should().Be(0x0080);
            materializedPacket.Location.Y.Should().Be(0x006D);
            materializedPacket.Location.Z.Should().Be(0);
            materializedPacket.ClickedOnType.Should().Be((ModelId)0x0EED);
        }

        [TestMethod]
        public void Can_deserialize_when_tile_targeted()
        {
            var rawPacket = FakePackets.Instantiate(source: new byte[]
            {
                0x6C, // packet
                0x01, // CursorTarget
                0xDE, 0xAD, 0xBE, 0xEF, // cursor id
                0x00, // CursorType
                0x00, 0x00, 0x00, 0x00, // clicked on id
                0x03, 0xE8, // xloc
                0x03, 0xE8, // yloc
                0x00, // unknown
                0x00, // zloc
                0x0D, 0x9B // type
            });

            var materializedPacket = new TargetCursorPacket();
            materializedPacket.Deserialize(rawPacket);

            materializedPacket.CursorTarget.Should().Be(CursorTarget.Location);
            materializedPacket.CursorId.Should().Be(new CursorId(0xDEADBEEF));
            materializedPacket.CursorType.Should().Be(CursorType.Neutral);
            materializedPacket.ClickedOnId.Should().Be(new ObjectId(0));
            materializedPacket.Location.X.Should().Be(0x03E8);
            materializedPacket.Location.Y.Should().Be(0x03E8);
            materializedPacket.Location.Z.Should().Be(0);
            materializedPacket.ClickedOnType.Should().Be((ModelId)0x0D9B);
        }
    }
}