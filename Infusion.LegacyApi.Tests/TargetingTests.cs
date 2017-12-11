using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Tests.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class TargetingTests
    {
        [TestMethod]
        public void Can_wait_for_target_When_no_last_action()
        {
            var testProxy = new InfusionTestProxy();
            var targeting = new Targeting(testProxy.Server, testProxy.Client, testProxy.Cancellation, new EventJournalSource());
            var task = Task.Run(() => targeting.WaitForTarget(TimeSpan.MaxValue));
            targeting.WaitForTargetStartedEvent.WaitOne(100).Should().BeTrue();
            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);

            task.Wait(100).Should().BeTrue();
        }

        [TestMethod]
        public void Can_wait_for_target_after_last_action_When_TargetCursor_arrives_before_waiting_starts()
        {
            var testProxy = new InfusionTestProxy();
            var targeting = new Targeting(testProxy.Server, testProxy.Client, 
                testProxy.Cancellation, new EventJournalSource());
            targeting.NotifyLastAction(DateTime.UtcNow.AddMilliseconds(-1));
            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);

            var task = Task.Run(() => targeting.WaitForTarget(TimeSpan.MaxValue));
            task.Wait(100).Should().BeTrue();
        }

        [TestMethod]
        public void Timeouts_when_waiting_for_target_and_TargetCursor_arrives_before_last_action()
        {
            var testProxy = new InfusionTestProxy();
            var targeting = new Targeting(testProxy.Server, testProxy.Client, testProxy.Cancellation, new EventJournalSource());

            testProxy.ServerPacketHandler.HandlePacket(TargetCursorPackets.TargetCursor);
            targeting.NotifyLastAction(DateTime.UtcNow.AddMilliseconds(1));

            var task = Task.Run(() => targeting.WaitForTarget(TimeSpan.MaxValue));
            task.Wait(100).Should().BeFalse();
        }
    }
}
