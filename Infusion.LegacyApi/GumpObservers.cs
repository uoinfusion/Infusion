using System;
using System.Text;
using System.Threading;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal sealed class GumpObservers
    {
        private readonly UltimaServer server;
        private readonly UltimaClient client;
        private readonly Legacy legacy;
        private readonly object gumpLock = new object();
        private readonly AutoResetEvent gumpReceivedEvent = new AutoResetEvent(false);
        private bool blockNextGumpMenuSelectionRequest;
        private bool showNextAwaitedGump = true;

        public GumpObservers(UltimaServer server, UltimaClient client, Legacy legacy)
        {
            this.server = server;
            this.client = client;
            this.legacy = legacy;
            server.RegisterFilter(FilterSendGumpMenuDialog);
            client.RegisterFilter(FilterGumpMenuSelection);
            client.Subscribe(PacketDefinitions.GumpMenuSelection, GumpMenuSelectionRequest);
        }

        public Gump CurrentGump { get; private set; }

        private void GumpMenuSelectionRequest(GumpMenuSelectionRequest packet)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null && packet.Id == CurrentGump.Id && packet.GumpId == CurrentGump.GumpId)
                    CurrentGump = null;
            }
        }

        private Packet? FilterGumpMenuSelection(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.GumpMenuSelection.Id && blockNextGumpMenuSelectionRequest)
            {
                blockNextGumpMenuSelectionRequest = false;
                return null;
            }

            return rawPacket;
        }

        private Packet? FilterSendGumpMenuDialog(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                var nextGumpNotVisible = false;

                lock (gumpLock)
                {
                    var packet = PacketDefinitionRegistry.Materialize<SendGumpMenuDialogPacket>(rawPacket);
                    CurrentGump = new Gump(packet.Id, packet.GumpId, packet.Commands, packet.TextLines);

                    if (!showNextAwaitedGump)
                    {
                        nextGumpNotVisible = true;
                        showNextAwaitedGump = true;
                    }
                }

                gumpReceivedEvent.Set();

                if (nextGumpNotVisible)
                    return null;
            }

            return rawPacket;
        }

        internal Gump WaitForGump(TimeSpan? timeout = null, bool showGump = true)
        {
            if (CurrentGump != null)
                return CurrentGump;

            showNextAwaitedGump = showGump;

            gumpReceivedEvent.Reset();

            var totalMilliseconds = 0;
            while (!gumpReceivedEvent.WaitOne(100))
            {
                totalMilliseconds += 100;
                if (timeout.HasValue && totalMilliseconds > timeout.Value.TotalMilliseconds)
                    return null;

                legacy.CheckCancellation();
            }
            return CurrentGump;
        }

        internal void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, TriggerGump).PushButton(buttonLabel, labelPosition)
                        .Execute();
                }
            }
        }

        internal void TriggerGump(GumpMenuSelectionRequest packet)
        {
            lock (gumpLock)
            {
                server.RequestGumpSelection(packet);

                blockNextGumpMenuSelectionRequest = true;
                client.CloseGump(CurrentGump.GumpId);
                CurrentGump = null;
            }
        }

        internal void TriggerGump(GumpControlId triggerId)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, TriggerGump).Trigger(triggerId)
                        .Execute();
                }
            }
        }

        internal void CloseGump()
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                    new GumpResponseBuilder(CurrentGump, TriggerGump).Cancel().Execute();
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
                builder.AppendLine($"Id {CurrentGump.Id:X8}, GumpId {CurrentGump.GumpId:X8}");
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