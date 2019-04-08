using System;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.LegacyApi.Tests.Packets;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.ItemManipulationTests
{
    [TestClass]
    public class WaitForItemDraggedTests
    {
        [TestMethod]
        public void WaitForItemDragged_waits_until_any_item_move_request_is_refused()
        {
            var testProxy = new InfusionTestProxy();
            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));
            testProxy.Api.WaitForItemDraggedStartedEvent.AssertWaitOneSuccess();
            testProxy.ServerPacketHandler.HandlePacket(RejectMoveItemRequestPackets.CannotLiftTheItem);

            task.AssertWaitFastSuccess();
        }

        [TestMethod]
        public void WaitForItemDragged_wait_until_the_dragged_object_is_deleted()
        {
            var testProxy = new InfusionTestProxy();
            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));
            testProxy.Api.WaitForItemDraggedStartedEvent.AssertWaitOneSuccess();
            testProxy.ServerPacketHandler.HandlePacket(new Packet(PacketDefinitions.DeleteObject.Id, new byte[]
            {
                0x1D, 0x12, 0x34, 0x56, 0x78
            }));

            task.AssertWaitFastSuccess();
        }

        [TestMethod]
        public void
            WaitForItemDragged_wait_when_other_object_than_dragged_object_is_deleted_the_dragged_object_is_deleted()
        {
            var testProxy = new InfusionTestProxy();
            var task = Task.Run(() => { testProxy.Api.WaitForItemDragged(0x78563412, TimeSpan.MaxValue); });
            testProxy.Api.WaitForItemDraggedStartedEvent.AssertWaitOneSuccess();
            testProxy.ServerPacketHandler.HandlePacket(new Packet(PacketDefinitions.DeleteObject.Id, new byte[]
            {
                0x1D, 0x12, 0x34, 0x56, 0x78
            }));

            task.Wait(100).Should().BeFalse();
        }

        [TestMethod]
        public void WaitForItemDragged_reacts_on_deleted_object_before_awaiting_starts_after_last_action()
        {
            var testProxy = new InfusionTestProxy();
            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));
            testProxy.ServerPacketHandler.HandlePacket(new Packet(PacketDefinitions.DeleteObject.Id, new byte[]
            {
                0x1D, 0x12, 0x34, 0x56, 0x78
            }));

            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));

            task.AssertWaitFastSuccess();
        }

        [TestMethod]
        public void WaitForItemDragged_ignores_deleted_object_before_last_action()
        {
            var testProxy = new InfusionTestProxy();
            testProxy.ServerPacketHandler.HandlePacket(new Packet(PacketDefinitions.DeleteObject.Id, new byte[]
            {
                0x1D, 0x12, 0x34, 0x56, 0x78
            }));
            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));

            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));

            task.Wait(100).Should().BeFalse();
        }

        [TestMethod]
        public void WaitForItemDragged_reacts_on_rejecting_move_request_before_awaiting_starts_after_last_action()
        {
            var testProxy = new InfusionTestProxy();
            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));
            testProxy.ServerPacketHandler.HandlePacket(RejectMoveItemRequestPackets.CannotLiftTheItem);

            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));

            task.AssertWaitFastSuccess();
        }

        [TestMethod]
        public void WaitForItemDragged_ignores_rejecting_move_request_before_last_action()
        {
            var testProxy = new InfusionTestProxy();
            testProxy.ServerPacketHandler.HandlePacket(RejectMoveItemRequestPackets.CannotLiftTheItem);
            testProxy.Api.NotifyAction(DateTime.UtcNow.AddMilliseconds(-1));

            var task = Task.Run(() => testProxy.Api.WaitForItemDragged(0x12345678, TimeSpan.MaxValue));

            task.Wait(100).Should().BeFalse();
        }
    }
}