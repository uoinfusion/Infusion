using System;
using FluentAssertions;
using FluentAssertions.Common;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Packets.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Gumps
{
    [TestClass]
    public class GumpResponseBuilderTests
    {
        [TestMethod]
        public void Can_create_response_for_button_in_front_of_requested_label()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}",
                new[] {"test label"});
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).PushButton("test label",
                GumpLabelPosition.Before).Execute();

            resultPacket.Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }

        [TestMethod]
        public void Can_create_response_for_button_in_front_of_two_identical_requested_labels()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495,
                "{Text 50 215 955 0}{Text 50 215 955 1}{Button 13 215 4005 4007 1 0 9}",
                new[] {"test label", "test label"});
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).PushButton("test label",
                GumpLabelPosition.Before).Execute();

            resultPacket.Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }

        [TestMethod]
        public void Can_create_response_with_selected_checkboxes()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x1F, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x02, // checkbox count
                0x00, 0x00, 0x00, 0x64, // checkbox1 id
                0x00, 0x00, 0x00, 0x67, // checkbox2 id
                0x00, 0x00, 0x00, 0x00
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495,
                "{Text 50 215 955 0}{CheckBox 13 57 9904 9903 0 100}{Text 50 215 955 1}{CheckBox 13 57 9904 9903 0 103}",
                new[] { "test label", "test label2" });

            new GumpResponseBuilder(gump, packet => { resultPacket = packet; })
                .SelectCheckBox("test label", GumpLabelPosition.Before)
                .SelectCheckBox("test label2", GumpLabelPosition.Before)
                .Trigger(0x09).Execute();

            resultPacket.Should().Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }

        [TestMethod]
        public void Can_create_response_with_textentry_content()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x1D, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x00, // checkbox count
                0x00, 0x00, 0x00, 0x01, // text entry count
                0x00, 0x05, // text entry id
                0x00, 0x01, // text entry content length
                0x00, 0x32, // unicode text entry content
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495,
                "{Text 50 215 955 0}{TextEntry 41 130 40 20 2301 5 10}",
                new[] { "test label" });

            new GumpResponseBuilder(gump, packet => { resultPacket = packet; })
                .SetTextEntry("test label", "2", GumpLabelPosition.Before)
                .Trigger(0x09).Execute();

            resultPacket.Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().IsSameOrEqualTo(expectedResponsePayload);

            // it seems that there is a bug in IsSameOrEqualTo
            for (int i = 0; i < resultPacket.RawPacket.Payload.Length; i++)
                resultPacket.RawPacket.Payload[i].Should().Be(expectedResponsePayload[i]);
        }

        [TestMethod]
        public void Can_create_response_for_button_after_requested_label()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x09, // selected trigger Id
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495, "{Button 13 215 4005 4007 1 0 9}{Text 50 215 955 0}",
                new[] {"test label"});
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).PushButton("test label",
                GumpLabelPosition.After).Execute();

            resultPacket.Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }

        [TestMethod]
        public void Can_create_failure_response_for_non_existent_label()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}", new[] {"test label"});
            var response = new GumpResponseBuilder(gump, packet => { }).PushButton("non existent label",
                GumpLabelPosition.Before);

            ((Action) response.Execute).ShouldThrow<GumpException>();
        }

        [TestMethod]
        public void Can_create_failure_response_for_requested_label_not_in_front_of_button()
        {
            var gump = new Gump(1, 2, "{Text 50 215 955 0}{Text 50 215 955 1}{Button 13 215 4005 4007 1 0 9}",
                new[] {"test label", "second not request label"});
            var response = new GumpResponseBuilder(gump, packet => { }).PushButton("non existent label",
                GumpLabelPosition.Before);

            ((Action) response.Execute).ShouldThrow<GumpException>();
        }

        [TestMethod]
        public void Can_create_cancel_response()
        {
            byte[] expectedResponsePayload =
            {
                0xB1, // packet
                0x00, 0x17, // packet length
                0x40, 0x00, 0x0D, 0xA7, // Id
                0x96, 0x00, 0x04, 0x95, // GumpId
                0x00, 0x00, 0x00, 0x00, // selected trigger Id - 0x00000000 for cancel
                0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            };

            GumpMenuSelectionRequest resultPacket = null;
            var gump = new Gump(0x40000DA7, 0x96000495, "{Text 50 215 955 0}{Button 13 215 4005 4007 1 0 9}",
                new[] {"test label"});
            new GumpResponseBuilder(gump, packet => { resultPacket = packet; }).Cancel().Execute();

            resultPacket.Should().NotBeNull();
            resultPacket.RawPacket.Payload.Should().BeEquivalentTo(expectedResponsePayload);
        }
    }
}