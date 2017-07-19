using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class BlockedPacketsFilters
    {
        public BlockedPacketsFilters(IServerPacketSubject serverPacketHandler)
        {
            serverPacketHandler.RegisterFilter(FilterBlockedPackets);
        }

        private Packet? FilterBlockedPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SetWeather.Id)
            {
                var newPacket = new SetWeatherPacket()
                {
                    NumberOfEffects = 0,
                    Type = WeatherType.None,
                    Temperature = 0,
                };

                return newPacket.RawPacket;
            }

            if (rawPacket.Id == PacketDefinitions.OverallLightLevel.Id)
            {
                var newPacket = new OverallLightLevelPacket()
                {
                    Level = 0,
                };

                return newPacket.RawPacket;
            }

            return rawPacket;
        }
    }
}
