using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;

namespace UltimaRX.Tests
{
    public static class AssertionExtensions
    {
        public static void AreEqual(this IEnumerable<Packet> expectedPackets, IEnumerable<Packet> actualPackets)
        {
            AreEqual(expectedPackets.ToArray(), actualPackets.ToArray());
        }

        public static void AreEqual(this Packet[] expectedPackets, Packet[] actualPackets)
        {
            Assert.AreEqual(expectedPackets.Length, actualPackets.Length);

            for (var i = 0; i < expectedPackets.Length; i++)
            {
                Assert.AreEqual(expectedPackets[i].Id, actualPackets[i].Id);
                Assert.AreEqual(expectedPackets[i].Length, actualPackets[i].Length);
                Assert.IsTrue(expectedPackets[i].Payload.SequenceEqual(actualPackets[i].Payload));
            }
        }
    }
}