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

        internal Gump WaitForGump(bool showGump = false)
        {
            if (CurrentGump != null)
                return CurrentGump;

            showNextAwaitedGump = showGump;

            gumpReceivedEvent.Reset();

            while (!gumpReceivedEvent.WaitOne(1000))
            {
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
    }
}