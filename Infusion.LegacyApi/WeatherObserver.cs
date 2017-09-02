using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class WeatherObserver
    {
        private readonly UltimaClient client;
        private readonly Configuration configuration;
        private Packet? lastWeatherPacket;

        public WeatherObserver(IServerPacketSubject serverPacketHandler, UltimaClient client, Configuration configuration)
        {
            this.client = client;
            this.configuration = configuration;
            serverPacketHandler.RegisterFilter(FilterBlockedServerPackets);
        }

        private Packet? FilterBlockedServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SetWeather.Id && configuration.FilterWeatherEnabled)
            {
                lastWeatherPacket = rawPacket.Clone();

                return CreateNeutralWeatherPacket();
            }

            return rawPacket;
        }

        public void ToggleWeatherFiltering()
        {
            if (configuration.FilterWeatherEnabled)
            {
                configuration.FilterWeatherEnabled = false;
                RestoreWeather();
            }
            else
            {
                configuration.FilterWeatherEnabled = true;
                client.Send(CreateNeutralWeatherPacket());
            }
        }

        private Packet CreateNeutralWeatherPacket() =>
            new SetWeatherPacket()
            {
                NumberOfEffects = 0,
                Type = WeatherType.None,
                Temperature = 0,
            }.RawPacket;

        private void RestoreWeather()
        {
            if (lastWeatherPacket.HasValue)
                client.Send(lastWeatherPacket.Value);
        }

    }
}