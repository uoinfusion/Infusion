using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests
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

        public static void AssertWaitOneSuccess(this EventWaitHandle ev)
        {
            ev.WaitOne(SlowTimeout).Should().BeTrue($"Unexpected WaitOne timeout {SlowTimeout}");
        }

        public static void WaitOneSlow(this EventWaitHandle ev)
        {
            ev.WaitOne(SlowTimeout);
        }

#if SLOW_MACHINE
        public static TimeSpan FastTimeout { get; } = TimeSpan.FromMilliseconds(2000);
#else
        public static TimeSpan FastTimeout { get; } = TimeSpan.FromMilliseconds(100);
#endif

        #if SLOW_MACHINE
        public static TimeSpan SlowTimeout { get; } = TimeSpan.FromMilliseconds(10000);
#else
        public static TimeSpan SlowTimeout { get; } = TimeSpan.FromMilliseconds(1000);
#endif
    }
}