using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class SpeechMessagePacketTests
    {
        [TestMethod]
        public void Can_deserialize_packet()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xAE, // packet
                0x00, 0x3C, // size
                0x00, 0x00, 0x00, 0x01, // ID
                0x01, 0x90, // Model
                0x03, // type
                0x02, 0xB2, // color
                0x00, 0x03, // font
                0x45, 0x4E, 0x55, 0x00, // language
                // name
                0x64, 0x64, 0x74, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00,
                0x61, 0x00, 0x73, 0x00, 0x64, 0x00, 0x66, 0x00, 0x00, // message
                0x13, 0x03, // ???
            });

            var packet = new SpeechMessagePacket();
            packet.Deserialize(rawPacket);

            packet.Id.Should().Be(0x00000001);
            packet.Model.Should().Be(0x0190);
            packet.Type.Should().Be((SpeechType)3);
            packet.Color.Should().Be((Color)0x02B2);
            packet.Font.Should().Be(0x0003);
            packet.Language.Should().Be("ENU");
            packet.Name.Should().Be("ddt");
            packet.Message.Should().Be("asdf");
        }
    }
}
