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
            var gumpResponseBuilder = new GumpResponseBuilder(currentGump, Program.SendToServer);
            gumpResponseBuilder.PushButton(buttonLabel).Execute();
        }
    }
}