using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Infusion.Tests
{
    [TestClass]
    public class UltimaClientConnectionTests_Post6060
    {
        [TestMethod]
        public void
    Given_connection_in_Initial_status_When_receives_login_seed_Then_connection_enters_ServerLogin_status()
        {
            var inputStream = new TestPullStream(new List<byte[]> { FakePackets.InitialLoginSeed_Post6060 });

            var connection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial);
            connection.ReceiveBatch(inputStream, inputStream.Length);

            connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
        }

        [TestMethod]
        public void
        Given_connection_in_Initial_status_When_receives_3_part_login_seed_Then_connection_enters_ServerLogin_status()
        {
            var inputStream = new TestPullStream(new List<byte[]> {
                new byte[] { 0xef },
                new byte[] { 0x01, 0x38, 0xa8, 0xc0 },
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x21, 0x00, 0x00, 0x00, 0x01,
                },
            });

            var connection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial);
            connection.ReceiveBatch(inputStream, 1);
            inputStream.NextBatch();

            connection.ReceiveBatch(inputStream, 4);
            inputStream.NextBatch();

            connection.ReceiveBatch(inputStream, 16);

            connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
        }

        [TestMethod]
        public void
Given_connection_in_Initial_status_When_receives_2_part_login_seed_second_part_appended_to_LoginRequest_Then_connection_enters_ServerLogin_status()
        {
            var inputStream = new TestPullStream(new List<byte[]> {
                new byte[] { 0xef },
                new byte[] { 0x01, 0x38, 0xa8, 0xc0 },
                new byte[]
                {
                    0x00, 0x00, 0x00, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x21, 0x00, 0x00, 0x00, 0x01,

                    0x80, 0x42, 0x75, 0x74, 0x68, 0x72, 0x6F, 0x6D, 0x79, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x4A,
                    0x46, 0x6B, 0x72, 0x34, 0x33, 0x21, 0x40, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF
                },
            });

            int packetsReceived = 0;

            var connection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial);
            connection.PacketReceived += (sender, packet) => packetsReceived++;

            connection.ReceiveBatch(inputStream, 1);
            inputStream.NextBatch();

            connection.ReceiveBatch(inputStream, 4);
            inputStream.NextBatch();

            connection.ReceiveBatch(inputStream, 78);
            inputStream.NextBatch();

            connection.Status.Should().Be(UltimaClientConnectionStatus.ServerLogin);
            packetsReceived.Should().Be(2);
        }
    }
}
