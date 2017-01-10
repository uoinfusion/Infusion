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
            packet.Type.Should().Be((ModelId)0x0EED);
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
            packet.Type.Should().Be((ModelId)0x372A);
            packet.Amount.Should().Be(1);
            packet.Location.X.Should().Be(0x129B);
            packet.Location.Y.Should().Be(0x0551);
            packet.Location.Z.Should().Be(0x0A);
            packet.Flags.Should().HaveFlag(ObjectFlag.None);
        }

        [TestMethod]
        public void Can_deserialize_packet_with_facing()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x1A, // packet
                0x00, 0x0F, // size
                0x40, 0x03, 0x24, 0xDD, // object id
                0x19, 0x8A, // type
                0x94, 0xEB, // xloc
                0x0B, 0xD0, // yloc
                0x01, // facing
                0x00, // zloc
            });

            var packet = new ObjectInfoPacket();
            packet.Deserialize(rawPacket);

            packet.Id.Should().Be(0x400324dd);
            packet.Type.Should().Be((ModelId)0x198a);
            packet.Location.X.Should().Be(0x14EB);
            packet.Location.Y.Should().Be(0x0bd0);
            packet.Location.Z.Should().Be(0x00);
        }

        [TestMethod]
        public void Can_deserialize_packet_with_dye()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x1A, // packet
                0x00, 0x10, // size
                0x40, 0x03, 0x7C, 0xBD, // object id
                0x1A, 0xD7, // type
                0x14, 0xF4, // xloc
                0x8B, 0xAE, // yloc
                0x06, // zloc
                0x09, 0x80 // dye
            });

            var packet = new ObjectInfoPacket();
            packet.Deserialize(rawPacket);

            packet.Id.Should().Be(0x40037cbd);
            packet.Type.Should().Be((ModelId)0x1ad7);
            packet.Location.X.Should().Be(0x14f4);
            packet.Location.Y.Should().Be(0x0BAE);
            packet.Location.Z.Should().Be(0x06);
            packet.Dye.Should().Be((Color) 0x0980);
        }
    }
}