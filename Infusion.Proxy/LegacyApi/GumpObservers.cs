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
            if (CurrentGump != null && packet.Id == CurrentGump.Id && packet.GumpId == CurrentGump.GumpId &&
                packet.TriggerId == 0)
            {
                CurrentGump = null;
            }
        }

        private Packet? FilterSendGumpMenuDialog(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SendGumpMenuDialogPacket>(rawPacket);
                CurrentGump = new Gump(packet.Id, packet.GumpId, packet.Commands, packet.TextLines);
                gumpReceivedEvent.Set();

                if (!showNextAwaitedGump)
                {
                    showNextAwaitedGump = true;
                    return null;
                }
            }

            return rawPacket;
        }

        internal Gump WaitForGump(TimeSpan? timeout = null, bool showGump = false)
        {
            if (CurrentGump != null)
                return CurrentGump;

            showNextAwaitedGump = showGump;

            gumpReceivedEvent.Reset();

            int totalMilliseconds = 0;
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
            if (CurrentGump != null)
            {
                new GumpResponseBuilder(CurrentGump, Program.SendToServer).PushButton(buttonLabel, labelPosition)
                    .Execute();
                CurrentGump = null;
            }
        }

        internal void TriggerGump(uint triggerId)
        {
            if (CurrentGump != null)
            {
                new GumpResponseBuilder(CurrentGump, Program.SendToServer).Trigger(triggerId)
                    .Execute();
                CurrentGump = null;
            }
        }

        internal void CloseGump()
        {
            if (CurrentGump != null)
                new GumpResponseBuilder(CurrentGump, Program.SendToServer).Cancel().Execute();
            CurrentGump = null;
        }

        public string GumpInfo()
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

        public GumpResponseBuilder GumpResponse()
        {
            if (CurrentGump != null)
                return new GumpResponseBuilder(CurrentGump, Program.SendToServer);

            return null;
        }
    }
}