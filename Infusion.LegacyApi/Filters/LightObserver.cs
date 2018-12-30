using Infusion.Packets;
using Infusion.Packets.Server;

namespace Infusion.LegacyApi.Filters
{
    public class LightObserver : ILightFilter
    {
        private readonly UltimaClient client;
        private readonly Player player;
        private readonly Legacy legacy;
        private readonly PacketDefinitionRegistry packetRegistry;
        private Packet? lastOverallLightLevelRawPacket;
        private bool enabled;

        internal LightObserver(IServerPacketSubject serverPacketHandler, UltimaClient client, Player player, Legacy legacy,
            PacketDefinitionRegistry packetRegistry)
        {
            this.client = client;
            this.player = player;
            this.legacy = legacy;
            this.packetRegistry = packetRegistry;
            serverPacketHandler.RegisterFilter(FilterBlockedServerPackets);
            serverPacketHandler.Subscribe(PacketDefinitions.PersonalLightLevel, HandlePersonalLightLevelPacket);
        }

        private void HandlePersonalLightLevelPacket(PersonalLightLevelPacket packet)
        {
            this.player.LightLevel = packet.Level;
        }

        private Packet? FilterBlockedServerPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.OverallLightLevel.Id && enabled)
            {
                lastOverallLightLevelRawPacket = rawPacket.Clone();
                var lastOverallLightLevelPacket =
                    packetRegistry.Materialize<OverallLightLevelPacket>(lastOverallLightLevelRawPacket
                        .Value);
                this.player.LightLevel = lastOverallLightLevelPacket.Level;

                return CreateFullLightLevelPacket();
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

        public void Enable()
        {
            enabled = true;
            client.Send(CreateFullLightLevelPacket());
            legacy.ClientPrint("Light filtering turned on");
        }

        public void Disable()
        {
            enabled = false;
            RestoreLight();
            legacy.ClientPrint("Light filtering turned off");
        }
    }
}