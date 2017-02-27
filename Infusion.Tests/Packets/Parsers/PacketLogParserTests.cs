using System;
using System.Linq;
using FluentAssertions;
using Infusion.Packets.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Parsers
{
    [TestClass]
    public class PacketLogParserTests
    {
        [TestMethod]
        public void Can_parse_proxy_to_server_packet()
        {
            var log =
                @"12:57:12.742 >>>> proxy -> server: RawPacket MoveRequest, length = 7
0x02, 0x81, 0x74, 0x00, 0x00, 0x00, 0x00, 
12:57:12.742 >>>> proxy -> server
0x61, 0xDA, 0x5D, 0x22, 0xD4, 0x35, 0x31, ";

            var parser = new PacketLogParser();
            var logEntry = parser.Parse(log).Single();

            FluentAssertions.AssertionExtensions.Should((TimeSpan) logEntry.Created).Be(TimeSpan.Parse("12:57:12.742"));
            FluentAssertions.AssertionExtensions.Should((object) logEntry.Direction).Be(PacketDirection.ProxyServer);
            FluentAssertions.AssertionExtensions.Should((string) logEntry.Name).Be("MoveRequest");
            logEntry.Payload.Should().BeEquivalentTo(new byte[] {0x02, 0x81, 0x74, 0x00, 0x00, 0x00, 0x00});
        }

        [TestMethod]
        public void Can_parse_server_to_proxy_packet()
        {
            var log =
                @"12:57:12.746 >>>> server -> proxy
0x08, 0xD9, 0xCE, 
12:57:12.746 >>>> server -> proxy: RawPacket CharacterMoveAck, length = 3
0x22, 0x74, 0x41, ";

            var parser = new PacketLogParser();
            var logEntry = parser.Parse(log).Single();

            FluentAssertions.AssertionExtensions.Should((TimeSpan) logEntry.Created).Be(TimeSpan.Parse("12:57:12.746"));
            FluentAssertions.AssertionExtensions.Should((object) logEntry.Direction).Be(PacketDirection.ServerProxy);
            FluentAssertions.AssertionExtensions.Should((string) logEntry.Name).Be("CharacterMoveAck");
            logEntry.Payload.Should().BeEquivalentTo(new byte[] {0x22, 0x74, 0x41});
        }

        [TestMethod]
        public void Can_parse_proxy_to_client_packet()
        {
            var log =
                @"12:57:12.746 >>>> proxy -> client: RawPacket DrawGamePlayer, length = 19
0x20, 0x00, 0x04, 0x5B, 0x2A, 0x01, 0x90, 0x00, 0x09, 0x09, 0x00, 0x09, 0x05, 0x0C, 0xF0, 0x00, 
12:57:12.746 >>>> proxy -> client
0xB4, 0xEB, 0x1C, 0x41, 0x7D, 0x70, 0xC4, 0xC4, 0x31, 0x51, 0x9D, 0xCF, 0x43, 0xD0, 0x68, ";

            var parser = new PacketLogParser();
            var logEntry = parser.Parse(log).Single();

            FluentAssertions.AssertionExtensions.Should((TimeSpan) logEntry.Created).Be(TimeSpan.Parse("12:57:12.746"));
            FluentAssertions.AssertionExtensions.Should((object) logEntry.Direction).Be(PacketDirection.ProxyClient);
            FluentAssertions.AssertionExtensions.Should((string) logEntry.Name).Be("DrawGamePlayer");
            logEntry.Payload.Should()
                .BeEquivalentTo(new byte[]
                    {0x20, 0x00, 0x04, 0x5B, 0x2A, 0x01, 0x90, 0x00, 0x09, 0x09, 0x00, 0x09, 0x05, 0x0C, 0xF0, 0x00});
        }

        [TestMethod]
        public void Can_parse_client_to_proxy_packet()
        {
            var log = @"12:57:12.937 >>>> client -> proxy
0x22, 0x00, 0x00, 
12:57:12.937 >>>> client -> proxy: RawPacket CharacterMoveAck, length = 3
0x22, 0x00, 0x00, ";

            var parser = new PacketLogParser();
            var logEntry = parser.Parse(log).Single();

            FluentAssertions.AssertionExtensions.Should((TimeSpan) logEntry.Created).Be(TimeSpan.Parse("12:57:12.937"));
            FluentAssertions.AssertionExtensions.Should((object) logEntry.Direction).Be(PacketDirection.ClientProxy);
            FluentAssertions.AssertionExtensions.Should((string) logEntry.Name).Be("CharacterMoveAck");
            logEntry.Payload.Should().BeEquivalentTo(new byte[] {0x22, 0x00, 0x00});
        }

        [TestMethod]
        public void Can_parse_packet_with_multiline_payload()
        {
            var log = @"12:57:12.937 >>>> proxy -> client: RawPacket DrawGamePlayer, length = 19
0x20, 0x00, 0x04, 0x5B, 0x2A, 0x01, 0x90, 0x00, 0x09, 0x09, 0x00, 0x09, 0x05, 0x0C, 0xF0, 0x00, 
0x00, 0x81, 0x00, 
12:57:12.937 >>>> proxy -> client
0xB4, 0xEB, 0x1C, 0x41, 0x7D, 0x70, 0xC4, 0xC4, 0x31, 0x51, 0x9D, 0xCF, 0x43, 0x08, 0x68, ";

            var parser = new PacketLogParser();
            var logEntry = parser.Parse(log).Single();

            FluentAssertions.AssertionExtensions.Should((TimeSpan) logEntry.Created).Be(TimeSpan.Parse("12:57:12.937"));
            FluentAssertions.AssertionExtensions.Should((object) logEntry.Direction).Be(PacketDirection.ProxyClient);
            FluentAssertions.AssertionExtensions.Should((string) logEntry.Name).Be("DrawGamePlayer");
            logEntry.Payload.Should().BeEquivalentTo(new byte[]
            {
                0x20, 0x00, 0x04, 0x5B, 0x2A, 0x01, 0x90, 0x00, 0x09, 0x09, 0x00, 0x09, 0x05, 0x0C, 0xF0, 0x00,
                0x00, 0x81, 0x00
            });
        }

        [TestMethod]
        public void Can_parse_multiple_packets()
        {
            var log = @"12:57:12.933 >>>> proxy -> server: RawPacket MoveRequest, length = 7
0x02, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 
12:57:12.933 >>>> proxy -> server
0x64, 0x5F, 0xC7, 0x63, 0x2D, 0x3F, 0x31, 

12:57:12.937 >>>> server -> proxy
0x09, 0x06, 0xD3, 
12:57:12.937 >>>> server -> proxy: RawPacket CharacterMoveAck, length = 3
0x22, 0x00, 0x41, ";

            var parser = new PacketLogParser();
            var entries = parser.Parse(log).ToArray();

            entries.Should().HaveCount(2);
            FluentAssertions.AssertionExtensions.Should((string) entries[0].Name).Be("MoveRequest");
            FluentAssertions.AssertionExtensions.Should((string) entries[1].Name).Be("CharacterMoveAck");
        }

        [TestMethod]
        public void Can_parse_packet_followed_by_new_line()
        {
            var log = @"12:57:12.933 >>>> proxy -> server: RawPacket MoveRequest, length = 7
0x02, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 
12:57:12.933 >>>> proxy -> server
0x64, 0x5F, 0xC7, 0x63, 0x2D, 0x3F, 0x31, 
";

            var parser = new PacketLogParser();
            var entries = parser.Parse(log).ToArray();

            entries.Should().HaveCount(1);
            FluentAssertions.AssertionExtensions.Should((string) entries[0].Name).Be("MoveRequest");
        }

        [TestMethod]
        public void Can_parse_packet_followed_by_several_new_lines()
        {
            var log = @"12:57:12.933 >>>> proxy -> server: RawPacket MoveRequest, length = 7
0x02, 0x81, 0x00, 0x00, 0x00, 0x00, 0x00, 
12:57:12.933 >>>> proxy -> server
0x64, 0x5F, 0xC7, 0x63, 0x2D, 0x3F, 0x31, 




";

            var parser = new PacketLogParser();
            var entries = parser.Parse(log).ToArray();

            entries.Should().HaveCount(1);
            FluentAssertions.AssertionExtensions.Should((string) entries[0].Name).Be("MoveRequest");
        }
    }
}