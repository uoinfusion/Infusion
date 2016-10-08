using System.Collections.Generic;
using System.Linq;
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

            AssertPackets(expectedPackets, receivedPackets.ToArray());
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

            AssertPackets(expectedPackets, receivedPackets.ToArray());
        }

        private void AssertPackets(Packet[] expectedPackets, Packet[] receivedPackets)
        {
            Assert.AreEqual(expectedPackets.Length, receivedPackets.Length);

            for (var i = 0; i < expectedPackets.Length; i++)
            {
                Assert.AreEqual(expectedPackets[i].Id, receivedPackets[i].Id);
                Assert.AreEqual(expectedPackets[i].Length, receivedPackets[i].Length);
                Assert.IsTrue(expectedPackets[i].Payload.SequenceEqual(receivedPackets[i].Payload));
            }
        }
    }
}