using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;

namespace UltimaRX.Tests
{
    [TestClass]
    public class UltimaClientConnectionTests
    {
        [TestMethod]
        public void Can_receive_tree_packets_in_one_batch()
        {
            var inputData = new List<byte[]>
            {
                FakePackets.InitialLoginSeed
                    .Concat(FakePackets.InitialLoginRequest)
                    .Concat(FakePackets.SelectServerRequest).ToArray()
            };

            var expectedPackets = new[]
            {
                new Packet(-1, FakePackets.InitialLoginSeed),
                new Packet(0x80, FakePackets.InitialLoginRequest),
                new Packet(0xA0, FakePackets.SelectServerRequest)
            };

            var connection = new UltimaClientConnection(inputData);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.ReceiveBatch();

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_receive_two_consecutive_batches()
        {
            var inputData = new List<byte[]>
            {
                FakePackets.InitialLoginSeed,
                FakePackets.InitialLoginRequest
            };

            var expectedPackets = new[]
            {
                new Packet(-1, FakePackets.InitialLoginSeed),
                new Packet(0x80, FakePackets.InitialLoginRequest)
            };

            var connection = new UltimaClientConnection(inputData);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.ReceiveBatch();
            connection.ReceiveBatch();

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_send_prelogin_packet()
        {
            var packet = FakePackets.Instantiate(FakePackets.GameServerList);
            var expectedSentBytes = FakePackets.GameServerList;
            var connection = new UltimaClientConnection(new[] {new byte[0]}, UltimaClientConnectionStatus.PreLogin);
            var outputStream = new TestMemoryStream();

            connection.Send(packet, outputStream);

            outputStream.ActualBytes.Should().BeEquivalentTo(expectedSentBytes);
        }

        [TestMethod]
        public void Can_send_game_packet()
        {
            var packet = FakePackets.Instantiate(FakePackets.EnableLockedClientFeatures);
            var expectedSentBytes = new byte[] { 0xB3, 0x32, 0x98, 0xDA };
            var connection = new UltimaClientConnection(new[] { new byte[0] }, UltimaClientConnectionStatus.Game);
            var outputStream = new TestMemoryStream();

            connection.Send(packet, outputStream);

            outputStream.ActualBytes.Should().BeEquivalentTo(expectedSentBytes);
        }
    }
}