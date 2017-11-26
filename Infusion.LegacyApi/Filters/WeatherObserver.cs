using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi.Filters
{
    internal class WeatherObserver : IWeatherFilter
    {
        private readonly UltimaClient client;
        private readonly Legacy legacy;
        private Packet? lastWeatherPacket;
        private bool enabled;

        public WeatherObserver(IServerPacketSubject serverPacketHandler, UltimaClient client, Legacy legacy)
        {
            this.client = client;
            this.legacy = legacy;
            serverPacketHandler.RegisterFilter(FilterBlockedServerPackets);
        }

        private Packet? FilterBlockedServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.SetWeather.Id && enabled)
            {
                lastWeatherPacket = rawPacket.Clone();

                return CreateNeutralWeatherPacket();
            }

            return rawPacket;
        }

        public void Toggle()
        {
            if (enabled)
            {
                Disable();
            }
            else
            {
                Enable();
            }
        }

        public void Enable()
        {
            enabled = true;
            client.Send(CreateNeutralWeatherPacket());
            legacy.ClientPrint("Weather filtering turned on");
        }

        public void Disable()
        {
            enabled = false;
            RestoreWeather();
            legacy.ClientPrint("Weather filtering turned off");
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