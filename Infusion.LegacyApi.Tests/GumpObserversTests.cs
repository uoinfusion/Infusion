using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Gumps;
using Infusion.LegacyApi.Tests.Packets;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class GumpObserversTests
    {
        [TestMethod]
        public void Can_wait_for_gump()
        {
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));
            Gump resultGump = null;

            var task = Task.Run(() => { resultGump = observer.WaitForGump(); });
            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();

            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel).Should().NotBeNull();

            task.Wait(100).Should().BeTrue();
            resultGump.Should().NotBeNull();
        }

        [TestMethod]
        public void Can_block_gump_for_client()
        {
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() => { observer.WaitForGump(false); });
            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();

            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel).Should().BeNull();

            task.Wait(100).Should().BeTrue();
        }

        [TestMethod]
        public void Doesnt_block_gump_for_client_after_blocked_gump()
        {
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() => { observer.WaitForGump(false); });
            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();

            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel).Should().BeNull();
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel).Should().NotBeNull();

            task.Wait(100).Should().BeTrue();
        }

        [TestMethod]
        public void Can_response_to_a_gump_without_provoking_unexpected_response_error_from_server()
        {
            // 1. proxy sends the response to server
            // 2. proxy sends close gump request to client (General information packet 0xBF, subcommand 0x04)
            // 3. game client responds to 0xBF/0x04 by sending gump cancellation request to proxy
            // 4. proxy blocks the gump cancellation request

            // If proxy doesn't block the gump cancellation request then server receives a response to a gump that was already
            // closed by proxy (step 1) and sends unexpected response error.

            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() =>
            {
                observer.WaitForGump();
                // cancellation a special case of gump response
                observer.GumpResponse().Cancel();
            });

            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            task.Wait(100).Should().BeTrue();
            testProxy.PacketReceivedFromClient(GumpMenuSelectionRequests.CancelExplevel).Should().BeNull();
        }

        [TestMethod]
        public void Doesnt_block_gump_response_from_client_after_closing_hidden_gump()
        {
            // 1. proxy sends the response to server
            // 2. proxy sends close gump request to client (General information packet 0xBF, subcommand 0x04)
            // 3. game client responds to 0xBF/0x04 by sending gump cancellation request to proxy
            // 4. proxy blocks the gump cancellation request
            // 5. user opens and cancells the same gump 

            // Proxy must not block the response in this case and the observer has to reset current gump.

            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() =>
            {
                observer.WaitForGump(false);
                observer.GumpResponse().Cancel();
            });

            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();

            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            task.Wait(100).Should().BeTrue();
            // script closes hidden gump,
            // but no cancellation request from game client, because there is no 0xBF/0x04 sent to the client

            // user opens gump from game client
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);

            // user closes gump from game client 
            testProxy.PacketReceivedFromClient(GumpMenuSelectionRequests.CancelExplevel);

            observer.CurrentGump.Should().BeNull();
        }

        [TestMethod]
        public void Doesnt_cancel_hidden_gump_on_game_client()
        {
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() =>
            {
                observer.WaitForGump(false); // next gump is hidden
                observer.GumpResponse().Cancel();
            });

            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();

            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            task.Wait(100).Should().BeTrue();

            testProxy.PacketsSentToClient.Should()
                .NotContain(packet => packet.Id == PacketDefinitions.GeneralInformationPacket.Id);
        }

        [TestMethod]
        public void Can_publish_event_after_receiving_gump()
        {
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));
            var journal = new EventJournal(testProxy.EventSource, new Cancellation(() => testProxy.CancellationTokenSource.Token));
            Gump resultGump = null;

            var task = Task.Run(() =>
            {
                journal.When<Events.GumpReceivedEvent>(e => resultGump = e.Gump)
                    .WaitAny();
            });

            journal.AwaitingStarted.WaitOne(100).Should().BeTrue();
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            task.Wait(100).Should().BeTrue();

            resultGump.Should().NotBeNull();
        }

        [TestMethod]
        public void Can_response_to_gump_and_receive_next_gump_from_server_before_game_client_sends_gump_cancallation_request()
        {
            // 1. proxy sends the response to the server
            // 2. proxy sends close gump request to the game client (General information packet 0xBF, subcommand 0x04)
            // 3. server sends a new gump to proxy
            // 3. game client responds to 0xBF/0x04 by sending gump cancellation request to proxy
            // 4. proxy blocks the gump cancellation request

            // This scenario can happen on slow computers, where server sends another gump before client responds with cancellation request
            // Server sends another gump after response quite frequently, e.g. crafting menus.
            // If proxy doesn't block the gump cancellation request then server receives a response to a gump that was already
            // closed by proxy (step 1) and sends unexpected response error.
            var testProxy = new InfusionTestProxy();
            var observer = new GumpObservers(testProxy.Server, testProxy.Client, testProxy.EventSource,
                new Cancellation(() => testProxy.CancellationTokenSource.Token));

            var task = Task.Run(() =>
            {
                observer.WaitForGump();
                // cancellation a special case of gump response
                observer.GumpResponse().Cancel();
            });

            observer.WaitForGumpStartedEvent.WaitOne(100).Should().BeTrue();
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            task.Wait(100).Should().BeTrue();

            // second gump from server comes before client cancellation of the first gump
            // first and second gump can have same ids
            testProxy.PacketReceivedFromServer(SendGumpMenuDialogPackets.Explevel);
            // game client cancellation of the first gump (client is slower than server)
            testProxy.PacketReceivedFromClient(GumpMenuSelectionRequests.CancelExplevel).Should().BeNull();
        }
    }
}