using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Proxy.InjectionApi
{
    internal class BlockedPacketsFilters
    {
        public BlockedPacketsFilters(ServerPacketHandler serverPacketHandler)
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
