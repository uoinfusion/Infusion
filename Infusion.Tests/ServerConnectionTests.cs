using System;
using System.Collections.Generic;
using FluentAssertions;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.IO.Encryption.Login;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests
{
    [TestClass]
    public class ServerConnectionTests
    {
        private PacketDefinitionRegistry packetRegistry;

        [TestInitialize]
        public void Initialize()
        {
            packetRegistry = PacketDefinitionRegistryFactory.CreateClassicClient();
        }

        [TestMethod]
        public void Can_receive_prelogin_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]> {FakePackets.GameServerList});

            var expectedPackets = new[] {new Packet(0xA8, FakePackets.GameServerList)};

            var connection = new ServerConnection();
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Process(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        [TestMethod]
        public void Can_write_diagnostic_info_about_received_packet()
        {
            var inputData = new MemoryBatchedStream(new List<byte[]>
            {
                new byte[] {0xB6, 0xA0, 0xFE, 0xE6}
            });

            var diagnosticStream = new TextDiagnosticPullStream(packetRegistry);
            var connection = CreateEncryptedConnection(ServerConnectionStatus.Game, diagnosticStream);
            connection.Process(inputData);

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

            var connection = CreateEncryptedConnection(ServerConnectionStatus.Game);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Process(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }

        private ServerConnection CreateEncryptedConnection(ServerConnectionStatus status,
            IDiagnosticPullStream diagnosticPullStream, IDiagnosticPushStream diagnosticPushStream)
        {
            return new ServerConnection(status, diagnosticPullStream,
                diagnosticPushStream, newGameKey: new byte[] { 127, 0, 0, 1 },
                loginSeed: 0xA9FE5050, loginKey: new LoginEncryptionKey(0x2cc3ed9d, 0xa374227f, 0));
        }

        private ServerConnection CreateEncryptedConnection(ServerConnectionStatus status)
            => CreateEncryptedConnection(status, NullDiagnosticPullStream.Instance, NullDiagnosticPushStream.Instance);

        private ServerConnection CreateEncryptedConnection(ServerConnectionStatus status, TextDiagnosticPullStream diagnosticStream)
            => CreateEncryptedConnection(status, diagnosticStream, NullDiagnosticPushStream.Instance);

        private ServerConnection CreateEncryptedConnection(ServerConnectionStatus status, TextDiagnosticPushStream diagnosticStream)
            => CreateEncryptedConnection(status, NullDiagnosticPullStream.Instance, diagnosticStream);

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

            connection.Process(inputData);

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
        public void Can_write_diagnostic_info_about_sent_Game_packet()
        {
            var diagnosticStream = new TextDiagnosticPushStream(packetRegistry);
            var connection = CreateEncryptedConnection(ServerConnectionStatus.Game, diagnosticStream);
            var testStream = new TestMemoryStream();
            connection.Send(FakePackets.GameServerLoginRequestPacket, testStream);

            var output = diagnosticStream.Flush();

            output.Should().Contain("0x91, 0x7F, 0x00, 0x00, 0x01, 0x61, 0x64")
                .And.Contain("0x22, 0x62, 0xD9, 0x50, 0x47, 0xA6, 0x00");
            
       }

        [TestMethod]
        public void Can_send_game_packet()
        {
            var connection = CreateEncryptedConnection(ServerConnectionStatus.Game);
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

            var connection = CreateEncryptedConnection(ServerConnectionStatus.Game);
            var receivedPackets = new List<Packet>();
            connection.PacketReceived += (sender, packet) => receivedPackets.Add(packet);
            connection.Process(inputData);

            expectedPackets.AreEqual(receivedPackets);
        }
    }
}