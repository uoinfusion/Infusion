using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Gumps;
using UltimaRX.Packets;

namespace UltimaRX.Tests.Gumps
{
    [TestClass]
    public class GumpResponseBuilderTests
    {
        [TestMethod]
        public void Can_create_response_for_button_in_front_of_requested_label()
        {
            byte[] expectedResponsePayload = {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
            };

            Packet? resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new [] { "test label" });
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).PushButton("test label").Execute();

            resultPacket.HasValue.Should().BeTrue();
            resultPacket?.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }

        [TestMethod]
        public void Can_create_failure_response_for_non_existent_label()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new[] { "test label" });
            var response = new GumpResponseBuilder(gump, packet => { }).PushButton("non existent label");

            ((Action)response.Execute).ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void Can_create_cancel_response()
        {
            byte[] expectedResponsePayload = {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x00, // selected trigger Id - 0x00000000 for cancel
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00,
            };

            Packet? resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new[] { "test label" });
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).Cancel().Execute();

            resultPacket.HasValue.Should().BeTrue();
            resultPacket?.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }
    }
}
