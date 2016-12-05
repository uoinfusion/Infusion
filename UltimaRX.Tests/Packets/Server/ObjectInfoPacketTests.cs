using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class ObjectInfoPacketTests
    {
        private static readonly Packet ObjectInfoPacketWithAmountWithFlag = FakePackets.Instantiate(source: new byte[]
        {
            0x1A, // packet
            0x00, 0x11, // size
            0xC0, 0x00, 0x00, 0x0B, // id
            0x0E, 0xED, // type
            0x01, 0xF4, // amount
            0x12, 0x8A, // xloc
            0x45, 0x42, // yloc
            0x0A, // zloc
            0x20 // flag
        });

        private static readonly Packet ObjectInfoPacket = FakePackets.Instantiate(source: new byte[]
        {
            0x1A, // packet
            0x00, 0x0E, // size
            0x40, 0x00, 0x00, 0x0F, // id
            0x37, 0x2A, // type
            0x12, 0x9B, // xloc
            0x05, 0x51, // yloc
            0x0A        // zloc
        });

        [TestMethod]
        public void Can_materialize_packet_with_amount()
        {
            var packet = new ObjectInfoPacket();
            packet.Deserialize(ObjectInfoPacketWithAmountWithFlag);

            packet.Id.Should().Be(0x4000000B);
            packet.Type.Should().Be(0x0EED);
            packet.Amount.Should().Be(0x01F4);
            packet.Location.X.Should().Be(0x128A);
            packet.Location.Y.Should().Be(0x0542);
            packet.Location.Z.Should().Be(0x0A);
            packet.Flags.Should().HaveFlag(ObjectFlag.Movable);
        }

        [TestMethod]
        public void Can_deserialize_packet_without_any_optional_part()
        {
            var packet = new ObjectInfoPacket();
            packet.Deserialize(ObjectInfoPacket);

            packet.Id.Should().Be(0x4000000F);
            packet.Type.Should().Be(0x372A);
            packet.Amount.Should().Be(1);
            packet.Location.X.Should().Be(0x129B);
            packet.Location.Y.Should().Be(0x0551);
            packet.Location.Z.Should().Be(0x0A);
            packet.Flags.Should().HaveFlag(ObjectFlag.None);
        }
    }
}