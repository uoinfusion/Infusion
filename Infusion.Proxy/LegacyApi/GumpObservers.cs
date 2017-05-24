using System;
using System.Text;
using System.Threading;
using Infusion.Gumps;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.Proxy.LegacyApi
{
    internal sealed class GumpObservers
    {
        private readonly object gumpLock = new object();
        private readonly AutoResetEvent gumpReceivedEvent = new AutoResetEvent(false);
        private bool showNextAwaitedGump = true;

        public GumpObservers(ServerPacketHandler serverPacketHandler, ClientPacketHandler clientPacketHandler)
        {
            serverPacketHandler.RegisterFilter(FilterSendGumpMenuDialog);
            clientPacketHandler.Subscribe(PacketDefinitions.GumpMenuSelection, GumpMenuSelectionRequest);
        }

        public Gump CurrentGump { get; private set; }

        private void GumpMenuSelectionRequest(GumpMenuSelectionRequest packet)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null && packet.Id == CurrentGump.Id && packet.GumpId == CurrentGump.GumpId &&
                    packet.TriggerId == 0)
                    CurrentGump = null;
            }
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

        internal Gump WaitForGump(TimeSpan? timeout = null, bool showGump = false)
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

                Legacy.CheckCancellation();
            }
            return CurrentGump;
        }

        internal void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, Program.SendToServer).PushButton(buttonLabel, labelPosition)
                        .Execute();
                    CurrentGump = null;
                }
            }
        }

        internal void TriggerGump(uint triggerId)
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                {
                    new GumpResponseBuilder(CurrentGump, Program.SendToServer).Trigger(triggerId)
                        .Execute();
                    CurrentGump = null;
                }
            }
        }

        internal void CloseGump()
        {
            lock (gumpLock)
            {
                if (CurrentGump != null)
                    new GumpResponseBuilder(CurrentGump, Program.SendToServer).Cancel().Execute();
                CurrentGump = null;
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
            if (CurrentGump != null)
                return new GumpResponseBuilder(CurrentGump, Program.SendToServer);

            return null;
        }
    }
}