using System;
using System.Text;
using System.Threading;
using Infusion.Gumps;
using Infusion.LegacyApi.Events;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class GumpObservers
    {
        private readonly UltimaServer server;
        private readonly UltimaClient client;
        private readonly EventJournalSource eventSource;
        private readonly Cancellation cancellation;
        private readonly PacketDefinitionRegistry packetRegistry;
        private readonly object gumpLock = new object();
        private readonly AutoResetEvent gumpReceivedEvent = new AutoResetEvent(false);
        private GumpTypeId? nextBlockedCancellationGumpId;
        private bool showNextAwaitedGump = true;
        private bool currentGumpVisible;

        internal AutoResetEvent WaitForGumpStartedEvent { get; } = new AutoResetEvent(false);

        public GumpObservers(UltimaServer server, UltimaClient client, EventJournalSource eventSource,
            Cancellation cancellation, PacketDefinitionRegistry packetRegistry)
        {
            this.server = server;
            this.client = client;
            this.eventSource = eventSource;
            this.cancellation = cancellation;
            this.packetRegistry = packetRegistry;
            server.RegisterFilter(FilterSendGumpMenuDialog);

            IClientPacketSubject clientPacketSubject = client;
            clientPacketSubject.RegisterFilter(FilterGumpMenuSelection);
            clientPacketSubject.Subscribe(PacketDefinitions.GumpMenuSelection, GumpMenuSelectionRequest);
        }

        public Gump CurrentGump { get; private set; }

        private void GumpMenuSelectionRequest(GumpMenuSelectionRequest packet)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null && packet.Id == CurrentGump.Id && packet.GumpTypeId == CurrentGump.GumpTypeId)
                    CurrentGump = null;
            }
        }

        private Packet? FilterGumpMenuSelection(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.GumpMenuSelection.Id && nextBlockedCancellationGumpId.HasValue)
            {
                lock (gumpLock)
                {
                    var packet = packetRegistry.Materialize<GumpMenuSelectionRequest>(rawPacket);
                    var gumpId = nextBlockedCancellationGumpId.Value;
                    nextBlockedCancellationGumpId = null;
                    if (gumpId == packet.GumpTypeId)
                        return null;
                }
            }

            return rawPacket;
        }

        private Packet? FilterSendGumpMenuDialog(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.GeneralInformationPacket.Id && rawPacket.Payload[4] == 4)
            {
                var packet = new CloseGenericGumpPacket();
                packet.Deserialize(rawPacket);

                eventSource.Publish(new ServerRequestedGumpCloseEvent(packet.GumpTypeId));
            }
            if (rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                var nextGumpNotVisible = false;

                lock (gumpLock)
                {
                    var packet = packetRegistry.Materialize<SendGumpMenuDialogPacket>(rawPacket);
                    var gump = new Gump(packet.GumpId, packet.GumpTypeId, packet.Commands, packet.TextLines);
                    CurrentGump = gump;
                    eventSource.Publish(new GumpReceivedEvent(gump));

                    if (!showNextAwaitedGump)
                    {
                        currentGumpVisible = false;
                        nextGumpNotVisible = true;
                        showNextAwaitedGump = true;
                    }
                    else
                        currentGumpVisible = true;
                }

                gumpReceivedEvent.Set();

                if (nextGumpNotVisible)
                    return null;
            }

            return rawPacket;
        }

        internal Gump WaitForGump(bool showGump = true, TimeSpan? timeout = null)
        {
            try
            {
                lock (gumpLock)
                {
                    if (CurrentGump != null)
                        return CurrentGump;

                    showNextAwaitedGump = showGump;

                    gumpReceivedEvent.Reset();
                    WaitForGumpStartedEvent.Set();
                }

                var totalMilliseconds = 0;
                while (!gumpReceivedEvent.WaitOne(100))
                {
                    totalMilliseconds += 100;
                    if (timeout.HasValue && totalMilliseconds > timeout.Value.TotalMilliseconds)
                        return null;

                    cancellation.Check();
                }
                return CurrentGump;
            }
            finally
            {
                showNextAwaitedGump = true;
            }
        }

        internal void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, TriggerGump).PushButton(buttonLabel, labelPosition);
                }
            }
        }

        internal void TriggerGump(GumpMenuSelectionRequest packet)
        {
            lock (gumpLock)
            {
                if (currentGumpVisible)
                {
                    nextBlockedCancellationGumpId = packet.GumpTypeId;
                    client.CloseGump(CurrentGump.GumpTypeId);
                }

                server.RequestGumpSelection(packet);
                CurrentGump = null;
            }
        }

        internal void TriggerGump(GumpControlId triggerId)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, TriggerGump).Trigger(triggerId);
                }
            }
        }

        internal void CloseGump()
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                    new GumpResponseBuilder(CurrentGump, TriggerGump).Cancel();
            }
        }

        public string LastGumpInfo()
        {
            lock (gumpLock)
            {
                if (CurrentGump == null)
                    return "no gump";

                var processor = new GumpParserDescriptionProcessor();
                var parser = new GumpParser(processor);
                parser.Parse(CurrentGump);

                var builder = new StringBuilder();
                builder.AppendLine($"Id {CurrentGump.Id}, GumpId {CurrentGump.GumpTypeId}");
                builder.AppendLine(CurrentGump.Commands);
                builder.AppendLine("-----------------");
                builder.AppendLine(processor.GetDescription());

                return builder.ToString();
            }
        }

        public GumpResponseBuilder GumpResponse()
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                    return new GumpResponseBuilder(CurrentGump, TriggerGump);

                throw new GumpException("No gump is currently available. If you see an active gump in game client it is most likely a bug. Please, type ,dump on Infusion command line and report the dump output on the Infusion project site.");
            }
        }
    }
}