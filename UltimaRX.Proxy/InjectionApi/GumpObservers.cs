using System.Text;
using System.Threading;
using UltimaRX.Gumps;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
{
    internal sealed class GumpObservers
    {
        private readonly AutoResetEvent gumpReceivedEvent = new AutoResetEvent(false);
        private Gump currentGump;
        private bool showNextAwaitedGump;
        private bool awaitingGump;

        public GumpObservers(ServerPacketHandler serverPacketHandler)
        {
            serverPacketHandler.RegisterFilter(FilterSendGumpMenuDialog);
        }

        private Packet? FilterSendGumpMenuDialog(Packet rawPacket)
        {
            if (awaitingGump && rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SendGumpMenuDialogPacket>(rawPacket);
                currentGump = new Gump(packet.Id, packet.GumpId, packet.Commands, packet.TextLines);
                gumpReceivedEvent.Set();

                if (!showNextAwaitedGump)
                    return null;
                showNextAwaitedGump = false;
            }

            return rawPacket;
        }

        internal Gump WaitForGump(bool showGump = false)
        {
            awaitingGump = true;
            showNextAwaitedGump = showGump;

            try
            {
                gumpReceivedEvent.Reset();

                while (!gumpReceivedEvent.WaitOne(1000))
                {
                    Injection.CheckCancellation();
                }
            }
            finally
            {
                awaitingGump = false;
            }
            return currentGump;
        }

        internal void SelectGumpButton(string buttonLabel, GumpLabelPosition labelPosition)
        {
            new GumpResponseBuilder(currentGump, Program.SendToServer).PushButton(buttonLabel, labelPosition).Execute();
        }

        internal void CloseGump()
        {
            new GumpResponseBuilder(currentGump, Program.SendToServer).Cancel().Execute();
        }

        public string GumpInfo()
        {
            WaitForGump(true);
            var gump = currentGump;
            if (gump == null)
                return "no gump";

            var processor = new GumpParserDescriptionProcessor();
            var parser = new GumpParser(processor);
            parser.Parse(gump);

            var builder = new StringBuilder();
            builder.AppendLine($"Id {gump.Id:X8}, GumpId {gump.GumpId:X8}");
            builder.AppendLine(gump.Commands);
            builder.AppendLine("-----------------");
            builder.AppendLine(processor.GetDescription());

            CloseGump();

            return builder.ToString();
        }
    }
}