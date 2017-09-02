using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi
{
    internal class LightObserver
    {
        private readonly UltimaClient client;
        private readonly Configuration configuration;
        private readonly Player player;
        private Packet? lastOverallLightLevelRawPacket;

        public LightObserver(IServerPacketSubject serverPacketHandler, UltimaClient client, Configuration configuration, Player player)
        {
            this.client = client;
            this.configuration = configuration;
            this.player = player;
            serverPacketHandler.RegisterFilter(FilterBlockedServerPackets);
            serverPacketHandler.Subscribe(PacketDefinitions.PersonalLightLevel, HandlePersonalLightLevelPacket);
        }

        private void HandlePersonalLightLevelPacket(PersonalLightLevelPacket packet)
        {
            this.player.LightLevel = packet.Level;
        }

        private Packet? FilterBlockedServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.OverallLightLevel.Id && configuration.FilterLightEnabled)
            {
                lastOverallLightLevelRawPacket = rawPacket.Clone();
                var lastOverallLightLevelPacket =
                    PacketDefinitionRegistry.Materialize<OverallLightLevelPacket>(lastOverallLightLevelRawPacket
                        .Value);
                this.player.LightLevel = lastOverallLightLevelPacket.Level;

                return CreateFullLightLevelPacket();
            }

            return rawPacket;
        }

        public void ToggleLightFiltering()
        {
            if (configuration.FilterLightEnabled)
            {
                configuration.FilterLightEnabled = false;
                RestoreLight();
            }
            else
            {
                configuration.FilterLightEnabled = true;
                client.Send(CreateFullLightLevelPacket());
            }
        }

        private Packet CreateFullLightLevelPacket() =>
            new OverallLightLevelPacket()
            {
                Level = 0,
            }.RawPacket;

        private void RestoreLight()
        {
            if (lastOverallLightLevelRawPacket.HasValue)
                client.Send(lastOverallLightLevelRawPacket.Value);
        }
    }
}