using Infusion.Packets;

namespace Infusion.LegacyApi
{
    internal class BlockedClientPacketsFilters
    {
        public BlockedClientPacketsFilters(IClientPacketSubject clientPacketHandler)
        {
            clientPacketHandler.RegisterFilter(FilterBlockedClientPackets);
        }

        private Packet? FilterBlockedClientPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.ResurrectionMenu.Id)
            {
                return null;
            }

            return rawPacket;
        }
    }
}
