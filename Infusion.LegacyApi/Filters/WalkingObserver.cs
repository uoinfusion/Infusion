using Infusion.Packets;

namespace Infusion.LegacyApi.Filters
{
    internal sealed class WalkingObserver : IWalkingFilter
    {
        private bool walkEnabled = true;
        private readonly UltimaClient client;
        private readonly Player player;

        internal WalkingObserver(IClientPacketSubject clientPacketSubject, UltimaClient client, Player player)
        {
            clientPacketSubject.RegisterFilter(FilterClientPackets);
            this.client = client;
            this.player = player;
        }

        private Packet? FilterClientPackets(Packet packet)
        {
            if (packet.Id == PacketDefinitions.MoveRequest.Id && !walkEnabled)
            {
                client.RejectMove(player.Location, player.Direction);
                return null;
            }

            return packet;
        }

        public void Disable()
        {
            walkEnabled = false;
        }

        public void Enable()
        {
            walkEnabled = true;
        }
    }
}
