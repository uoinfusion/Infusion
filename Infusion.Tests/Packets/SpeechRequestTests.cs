using FluentAssertions;
using Infusion.Packets.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets
{
    [TestClass]
    public class SpeechRequestTests
    {
        [TestMethod]
        public void Can_deserialize_packet_with_keywords()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xAD, // packet
                0x00, 0x15, // size
                0xC0, // type
                0x02, 0xB2, // color
                0x00, 0x03, // font
                0x45, 0x4E, 0x55, 0x00, // language
                0x00, 0x10, 0x1C, // keywords
                0x2C, 0x74, 0x65, 0x73, 0x74, 0x00, // message
            });

            var packet = new SpeechRequest();
            packet.Deserialize(rawPacket);

            packet.Text.Should().Be(",test");
        }

        [TestMethod]
        public void Can_deserialize_unicode()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xAD, // packet
                0x00, 0x16, // size
                0x00, // type
                0x02, 0xB2, // color
                0x00, 0x03, // font
                0x45, 0x4E, 0x55, 0x00, // language
                0x00, 0x61, 0x00, 0x73, 0x00, 0x64, 0x00, 0x66, 0x00, 0x00, // message
            });

            var packet = new SpeechRequest();
            packet.Deserialize(rawPacket);

            packet.Text.Should().Be("asdf");
        }
    }
}
