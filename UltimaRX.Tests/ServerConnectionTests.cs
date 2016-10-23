using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Tests
{
    [TestClass]
    public class ServerConnectionTests
    {
        [TestMethod]
        public void Can_receive_prelogin_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]> {FakePackets.GameServerList});

            var expectedPackets = new[] {new Packet(0xA8, FakePackets.GameServerList)};

            var connection = new ServerConnection();
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_received_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xE6}
            });


            var diagnosticStream = new DiagnosticPullStream();
            var connection = new ServerConnection(ServerConnectionStatus.Game, diagnosticStream,
                NullDiagnosticPushStream.Instance);
            connection.Receive(inputData);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0xB6, 0xA0, 0xFE, 0xE6")
                .And.Contain("0xB9, 0x80, 0x1F");
        }

        [TestMethod]
        public void Can_receive_one_game_packets()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xF9}
            });
            var expectedPackets = new[] {new Packet(0xB9, FakePackets.EnableLockedClientFeatures)};

            var connection = new ServerConnection(ServerConnectionStatus.Game, NullDiagnosticPullStream.Instance,
                NullDiagnosticPushStream.Instance);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_send_prelogin_packet()
        {
            var connection = new ServerConnection(ServerConnectionStatus.PreLogin);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.InitialLoginRequest), testStream);

            testStream.ActualBytes.Should().BeEquivalentTo(FakePackets.InitialLoginRequestEncrypted);
        }

        [TestMethod]
        public void Given_connection_in_Initial_status_When_sends_login_seed_Then_enters_PreLogin_status()
        {
            var connection = new ServerConnection(ServerConnectionStatus.Initial);
            connection.Send(FakePackets.InitialLoginSeedPacket, new TestMemoryStream());

            connection.Status.Should().Be(ServerConnectionStatus.PreLogin);
        }

        [TestMethod]
        public void Given_connection_in_PreLogin_status_When_receives_ConnectToGameServer_Then_enters_PreGame_status()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]> { FakePackets.ConnectToGameServer });
            var connection = new ServerConnection(ServerConnectionStatus.PreLogin);

            connection.Receive(inputData);

            connection.Status.Should().Be(ServerConnectionStatus.PreGame);
        }

        [TestMethod]
        public void Given_connection_in_PreGame_status_When_sends_login_seed_Then_enters_Game_status()
        {
            var connection = new ServerConnection(ServerConnectionStatus.PreGame);
            connection.Send(FakePackets.LoginSeedPacket, new TestMemoryStream());

            connection.Status.Should().Be(ServerConnectionStatus.Game);
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_sent_PreLogin_packet()
        {
            var diagnosticStream = new DiagnosticPushStream();

            var connection = new ServerConnection(ServerConnectionStatus.PreLogin, NullDiagnosticPullStream.Instance,
                diagnosticStream);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.InitialLoginRequest), testStream);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0x80, 0x61, 0x64, 0x6D, 0x69, 0x6E")
                .And.Contain("0x7A, 0x63, 0x9A, 0xED, 0x56, 0x0E");
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_sent_Game_packet()
        {
            var diagnosticStream = new DiagnosticPushStream();
            var connection = new ServerConnection(ServerConnectionStatus.Game, NullDiagnosticPullStream.Instance,
                diagnosticStream);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.GameServerLoginRequestPacket, testStream);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0x91, 0x7F, 0x00, 0x00, 0x01, 0x61, 0x64")
                .And.Contain("0x22, 0x62, 0xD9, 0x50, 0x47, 0xA6, 0x00");
            
       }

        [TestMethod]
        public void Can_send_game_packet()
        {
            var connection = new ServerConnection(ServerConnectionStatus.Game, NullDiagnosticPullStream.Instance,
                NullDiagnosticPushStream.Instance);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.Instantiate(FakePackets.GameServerLoginRequest), testStream);

            testStream.ActualBytes.Should().BeEquivalentTo(FakePackets.GameServerLoginRequestEncrypted);
        }

        [TestMethod]
        public void Can_send_PreGame_packet()
        {
            var connection = new ServerConnection(ServerConnectionStatus.PreGame, NullDiagnosticPullStream.Instance,
                NullDiagnosticPushStream.Instance);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.LoginSeedPacket, testStream);

            testStream.ActualBytes.Should().BeEquivalentTo(FakePackets.LoginSeed);
        }

        [TestMethod]
        public void Can_receive_two_Game_packets()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                FakePackets.EnableLockedClientFeaturesEncrypted,
                FakePackets.CharactersStartingLocationsEncrypted
            });

            var expectedPackets = new[]
            {
                new Packet(0xB9, FakePackets.EnableLockedClientFeatures),
                new Packet(PacketDefinitions.CharactersStartingLocations.Id, FakePackets.CharactersStartingLocations), 
            };

            var connection = new ServerConnection(ServerConnectionStatus.Game);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Receive(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }
    }
}