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
            var connection = new UltimaClientConnection(new[] {new byte[0]}, UltimaClientConnectionStatus.ServerLogin);
            var outputStream = new TestMemoryStream();

            connection.Send(packet, outputStream);

            outputStream.ActualBytes.Should().BeEquivalentTo(expectedSentBytes);
        }

        [TestMethod]
        public void Can_send_game_packet()
        {
            var packet = FakePackets.Instantiate(FakePackets.EnableLockedClientFeatures);
            var expectedSentBytes = new byte[] {0xB3, 0x32, 0x98, 0xDA};
            var connection = new UltimaClientConnection(new[] {new byte[0]}, UltimaClientConnectionStatus.Game);
            var outputStream = new TestMemoryStream();

            connection.Send(packet, outputStream);

            outputStream.ActualBytes.Should().BeEquivalentTo(expectedSentBytes);
        }

        [TestMethod]
        public void
            Given_connection_in_Initial_status_When_receives_login_seed_Then_connection_enters_ServerLogin_status()
        {
            var inputData = new List<byte[]> {FakePackets.InitialLoginSeed};

            var connection = new UltimaClientConnection(inputData, UltimaClientConnectionStatus.Initial);
            connection.ReceiveBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
        }

        [TestMethod]
        public void
            Given_connection_in_ServerLogin_status_When_receives_SelectServerRequest_Then_enters_PreGameLogin_status()
        {
            var inputData = new List<byte[]> {FakePackets.SelectServerRequest};

            var connection = new UltimaClientConnection(inputData, UltimaClientConnectionStatus.ServerLogin);
            connection.ReceiveBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.PreGameLogin);
        }

        [TestMethod]
        public void
            Given_connection_in_PreGameLogin_status_When_receives_login_seed_Then_connection_enters_GameLogin_status()
        {
            var inputData = new List<byte[]> {FakePackets.LoginSeed};

            var connection = new UltimaClientConnection(inputData, UltimaClientConnectionStatus.PreGameLogin);
            connection.ReceiveBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.GameLogin);
        }

        [TestMethod]
        public void Given_connection_in_GameLoign_status_When_receives_GameServerLoginRequest_Then_enters_Game_status()
        {
            var inputData = new List<byte[]> {FakePackets.GameServerLoginRequest};

            var connection = new UltimaClientConnection(inputData, UltimaClientConnectionStatus.GameLogin);
            connection.ReceiveBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.Game);
        }

        [TestMethod]
        public void Can_enter_Game_status_after_initial_sequence()
        {
            var inputData = new List<byte[]>
            {
                FakePackets.InitialLoginSeed,
                FakePackets.InitialLoginRequest,
                FakePackets.ClientSpy,
                FakePackets.SelectServerRequest,
                FakePackets.LoginSeed,
                FakePackets.GameServerLoginRequest
            };

            var connection = new UltimaClientConnection(inputData, UltimaClientConnectionStatus.Initial);
            connection.ReceiveBatch();
            connection.ReceiveBatch();
            connection.ReceiveBatch();
            connection.ReceiveBatch();
            connection.ReceiveBatch();
            connection.ReceiveBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.Game);
        }
    }
}