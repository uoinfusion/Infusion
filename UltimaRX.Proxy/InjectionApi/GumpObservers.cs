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
        private bool waitingForGump;

        public GumpObservers(ServerPacketHandler serverPacketHandler)
        {
            serverPacketHandler.RegisterFilter(FilterSendGumpMenuDialog);
        }

        private Packet? FilterSendGumpMenuDialog(Packet rawPacket)
        {
            if (waitingForGump && rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<SendGumpMenuDialogPacket>(rawPacket);
                currentGump = new Gump(packet.Id, packet.GumpId, packet.Commands, packet.TextLines);
                gumpReceivedEvent.Set();

                return null;
            }

            return rawPacket;
        }

        internal Gump WaitForGump()
        {
            waitingForGump = true;

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
                waitingForGump = false;
            }
            return currentGump;
        }

        internal void SelectGumpButton(string buttonLabel)
        {
            new GumpResponseBuilder(currentGump, Program.SendToServer).PushButton(buttonLabel).Execute();
        }

        internal void CloseGump()
        {
            new GumpResponseBuilder(currentGump, Program.SendToServer).Cancel().Execute();
        }

        public string GumpInfo()
        {
            WaitForGump();
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